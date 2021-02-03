using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{

    //provisory
    public KeyCode leftKey;
    public KeyCode rightKey;
    //-----------------------------

    //Starting position depends on number of player. Set by initial interface.
    [SerializeField]
    private Vector2 nextCoordinates = new Vector2(3, 3);
    //Can be modified during the countdown before the match
    [SerializeField]
    private Vector2 nextDirection = Vector2.up;

    private Vector3 nextWorldPosition;

    
    [SerializeField]
    private float baseSpeed = 7;
    private float speed
    {
        get
        {
            //add the modifiers for the special body parts
            return Mathf.Clamp(baseSpeed - bodyParts.Count * 0.5f, 3, 20);
        }
    }
    private float moveRatio = 0;

    [SerializeField]
    private List<SnakeBodyPart> bodyParts = new List<SnakeBodyPart>();
    public SnakeBodyPart head { get { return bodyParts[0]; } }
    public SnakeBodyPart tail { get { return bodyParts[bodyParts.Count-1]; } }

    private bool isDead = false;

    private void Awake()
    {
        if (bodyParts == null) bodyParts = new List<SnakeBodyPart>();
    }

    void Start()
    {
        //just for testing, this function should be called by the GameController
        StartSetup(nextCoordinates, nextDirection);
        BodySetup();
    }

    public void StartSetup(Vector2 coordinates, Vector2 direction)
    {
        while (bodyParts.Count < Random.Range(5,20))
        {
            string url;
            //the initial body formation will be create by the player (3 parts)
            if (bodyParts.Count == 0) url = BatteringRamPart.PREFAB_URL;
            else url = SnakeBodyPart.PREFAB_URL;
            SnakeBodyPart p = Instantiate(Resources.Load<SnakeBodyPart>(url));
            p.transform.SetParent(transform);
            bodyParts.Add(p);
        }

        nextCoordinates = coordinates + direction;
        nextWorldPosition = GridSlot(nextCoordinates);

        Vector3 pos;
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i == 0)
            {
                head.coordinates = coordinates;

                pos = GridSlot(head.coordinates);
                head.worldPosition = pos;
                head.transform.position = pos;

                GridSnake.instance.PlaceOnGrid(head, coordinates);
            }
            else
            {
                bodyParts[i].coordinates = head.coordinates;
                bodyParts[i].transform.position = head.worldPosition;
                bodyParts[i].worldPosition = head.worldPosition;
            }
        }
    }

    public void BodySetup() {
        float ratio = 0.4f / bodyParts.Count;
        float scale = 1;
        for (int i = 0; i< bodyParts.Count; i++)
        {
            bodyParts[i].name = "Snake part: " + i;
            bodyParts[i].transform.localScale = Vector3.one * scale;
            scale -= ratio;
        }
    }

   
    private void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(leftKey)) Turn(-1); //nextInput = -1;
        else if (Input.GetKeyDown(rightKey)) Turn(1); //nextInput = 1;

        if (head.coordinates != nextCoordinates)
        {
            if (moveRatio >= 0.5f)//if body parts changed gridslot
                UpdateGridReferences();
        }
        else if (moveRatio >= 1)
        {
            ChooseNextDestination();
            moveRatio = 0;
        }
        MoveBody();
    }

    private void MoveBody()
    {
        moveRatio += speed * Time.deltaTime;

        for (int i = bodyParts.Count-1; i >= 0 ; i--)
        {
            if (i == 0)
            {
                head.transform.position =
                   Vector3.Lerp(head.worldPosition,
                                   nextWorldPosition,
                                   moveRatio);
            }
            else
            {
                Vector3 p1 = bodyParts[i].worldPosition;
                Vector3 p2 = bodyParts[i - 1].worldPosition;

                if (p1 == p2) continue;

                bodyParts[i].transform.position =
                    Vector3.Lerp(p1, p2, moveRatio);
            }
        }
    }
    private void ChooseNextDestination()
    {
        for (int i = bodyParts.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                head.orientation = nextDirection;
            else
                bodyParts[i].orientation = bodyParts[i - 1].orientation;
        }

        nextCoordinates = head.coordinates + head.orientation;

        UpdateWorldReferences();
    }
    private void UpdateWorldReferences()
    {
        for (int i = bodyParts.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                head.worldPosition = nextWorldPosition;
            else
                bodyParts[i].worldPosition = bodyParts[i - 1].worldPosition;
        }

        nextWorldPosition = GridSlot(nextCoordinates);
    }
    private void UpdateGridReferences()
    {
        for (int i = bodyParts.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                GridSnake.instance.PlaceOnGrid(head, nextCoordinates);
            else
            {
                if (bodyParts[i].coordinates == bodyParts[i - 1].coordinates)
                    continue;
                
                GridSnake.instance.PlaceOnGrid(bodyParts[i],
                                    bodyParts[i - 1].coordinates);
            }
        }
    }

    public bool IsFromSnake(Entity entity)
    {
        if (entity is SnakeBodyPart)
            return bodyParts.Contains((SnakeBodyPart)entity);
        return false;
    }

    private void Turn(int value)
    {
        //should lock the first input?
        if (value != 0)
        {
            if (head.orientation.x != 0) nextDirection.x = 0;
            else { nextDirection.x = head.orientation.y * value; }
            if (head.orientation.y != 0) nextDirection.y = 0;
            else { nextDirection.y = -head.orientation.x * value; }
        }
    }

    public void AddBodyPart(string url)
    {
        SnakeBodyPart p = Instantiate( Resources.Load<SnakeBodyPart>(url) );
        p.transform.SetParent(transform);

        if (url.Equals(SnakeBodyPart.PREFAB_URL) )
        {
            p.orientation = tail.orientation;
            GridSnake.instance.PlaceOnGrid(p, tail.coordinates);
            p.worldPosition = tail.worldPosition;
            bodyParts.Add(p);

            p.transform.position = p.worldPosition;
        }
        else
        {
            p.orientation = head.orientation;
            GridSnake.instance.PlaceOnGrid(p, head.coordinates);
            p.worldPosition = GridSlot(p.coordinates);
            bodyParts.Insert(0, p);


            nextCoordinates = head.coordinates + head.orientation;
            nextWorldPosition = GridSlot(nextCoordinates);

            head.transform.position =
               Vector3.Lerp(head.worldPosition,
                               nextWorldPosition,
                               moveRatio);
        }
    }

    public void Eat(Pickable food)
    {
        /*
         * if(food == player.food){
         *  AddBodyPart(food.partURL);
         *  add many points
         * }
         * else {
         *  AddBodyPart(SnakeBodyPart.PREFAB_URL);
         *  add few points
         *  add velocity debuff
         * }
         * 
        */
        AddBodyPart(food.partURL);
    }

    public void RemovePart(int index)
    {
        if (index >= bodyParts.Count) return;

        if (index != bodyParts.Count - 1)
        {
            for (int i = bodyParts.Count - 1; i > index; i--)
            {
                Vector2 coords1 = bodyParts[i].coordinates;
                Vector2 coords2 = bodyParts[i - 1].coordinates;

                if(coords1 != coords2)
                    GridSnake.instance.PlaceOnGrid(bodyParts[i],
                                bodyParts[i - 1].coordinates);

                bodyParts[i].orientation = bodyParts[i - 1].orientation;
                bodyParts[i].worldPosition = bodyParts[i - 1].worldPosition;
                bodyParts[i].transform.position = bodyParts[i - 1].transform.position;
               
            }
        }
        GridSnake.instance.RemoveFromGrid(bodyParts[index]);
        Destroy(bodyParts[index].gameObject);
        bodyParts.RemoveAt(index);
    }

    public void UseHeadPower()
    {
        RemovePart(0);
        AddBodyPart(SnakeBodyPart.PREFAB_URL);
        BodySetup();
    }

    public void Death()
    {
        if (isDead) return;
        isDead = true;
        StartCoroutine("DeathAnimation");
    }
    private IEnumerator DeathAnimation()
    {
        yield return null;

        foreach (SnakeBodyPart part in bodyParts)
            GridSnake.instance.RemoveFromGrid(part);

        while (head.transform.localScale.x > 0.1f)
        {
            foreach (SnakeBodyPart part in bodyParts)
            {
                if(part.transform.localScale.x > 0.1f)
                    part.transform.localScale -= Vector3.one * 0.1f;
            }

            yield return null;
        }

        gameObject.SetActive(false);
        
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Auxiliary function: Converts the coordinates to world position
    /// using GridSnake functions.
    /// **GridSnake.instance.getWorldPositionOfSlot(coords);**
    /// </summary>
    private Vector3 GridSlot(Vector2 coords)
    { return GridSnake.instance.getWorldPositionOfSlot(coords); }


    private void OnDrawGizmos()
    {
        if (bodyParts == null || bodyParts.Count == 0) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(head.transform.position,
            head.transform.position+new Vector3(nextDirection.x,0,nextDirection.y));
    }
}
