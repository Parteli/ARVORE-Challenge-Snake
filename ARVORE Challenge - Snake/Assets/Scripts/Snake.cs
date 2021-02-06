using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnakeController))]
public class Snake : MonoBehaviour
{
    #region Movement Properties

    //Starting position depends on number of player. Set by initial interface.
    [SerializeField]
    public Vector2 nextCoordinates = new Vector2(3, 3);
    //Can be modified during the countdown before the match
    [SerializeField]
    public Vector2 nextDirection = Vector2.up;

    public Vector3 nextWorldPosition;

    //Speed section------

    private static float MAX_SPEED = 20;
    private static float MIN_SPEED = 1;
    private static float BASE_SPEED = 10;
    private static float WEIGHT_RATIO = 0.4f;
    private float currentSpeed
    {
        get
        {
            float s = BASE_SPEED - bodyList.Count * WEIGHT_RATIO + speedBuff;
            //add the modifiers for the special body parts
            return Mathf.Clamp(s, MIN_SPEED, MAX_SPEED);
        }
    }
    private float speedBuff;
    private float moveRatio = 0;

    #endregion

    #region Body and General Properties
    [SerializeField]
    private List<SnakeBodyPart> bodyList = new List<SnakeBodyPart>();
    //stores the initial setup and later is used to add parts at the tail
    [HideInInspector]
    public List<string> spareList = new List<string>();
    public SnakeBodyPart head { get { return bodyList[0]; } }
    public SnakeBodyPart tail { get { return bodyList[bodyList.Count-1]; } }

    public Pickable food;

    [HideInInspector] public Color color;

    [HideInInspector] public bool isDead = false;
    #endregion

    #region Start and States

    private void Awake()
    {
        if (bodyList == null) bodyList = new List<SnakeBodyPart>();
    }

    public void StartSetup(Vector2 coordinates, Vector2 direction)
    {

        for (int i = 0; i < spareList.Count; i++)
        {
            SnakeBodyPart p = Instantiate(Resources.Load<SnakeBodyPart>(spareList[i]));
            p.transform.SetParent(transform);
            bodyList.Add(p);

            p.orientation = direction;
            p.worldPosition = GridSlot(coordinates);
            p.transform.position = p.worldPosition;

            if (i == 0) GridSnake.instance.PlaceOnGrid(p, coordinates);
            else p.coordinates = coordinates;

            Utility.SetColorForAll(p.transform, color);
        }

        spareList.Clear();

        nextDirection = direction;
        nextCoordinates = coordinates + direction;
        nextWorldPosition = GridSlot(nextCoordinates);

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

        foreach (SnakeBodyPart part in bodyList)
            GridSnake.instance.RemoveFromGrid(part);

        while (head.transform.localScale.x > 0.1f)
        {
            foreach (SnakeBodyPart part in bodyList)
            {
                if (part.transform.localScale.x > 0.1f)
                    part.transform.localScale -= Vector3.one * 0.1f;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }


    #endregion

    #region Movement

    public void CustomUpdate()
    {
        if (isDead) return;


        if (head.coordinates != nextCoordinates)
        {
            if (moveRatio >= 0.5f)//if body parts changed gridslot
                UpdateGridReferences();//updates coordinates
        }
        if (moveRatio >= 1)
        {
            ChooseNextDestination();//updates orientations
            UpdateWorldReferences();//updates worldpositions

            if (spareList.Count > 0) AddPartAtEnd();

            moveRatio -= 1;
        }
        MoveBody();
        moveRatio += currentSpeed * Time.deltaTime;
    }

    private void MoveBody()
    {
        for (int i = bodyList.Count - 1; i >= 0; i--)
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
                Vector3 p1 = bodyList[i].worldPosition;
                Vector3 p2 = bodyList[i - 1].worldPosition;

                if (p1 == p2) continue;

                bodyList[i].transform.position =
                    Vector3.Lerp(p1, p2, moveRatio);
            }
        }
    }
    private void ChooseNextDestination()
    {
        for (int i = bodyList.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                head.orientation = nextDirection;
            else
                bodyList[i].orientation = bodyList[i - 1].orientation;
        }

        nextCoordinates = head.coordinates + head.orientation;
    }
    private void UpdateWorldReferences()
    {
        for (int i = bodyList.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                head.worldPosition = nextWorldPosition;
            else
                bodyList[i].worldPosition = bodyList[i - 1].worldPosition;
        }

        nextWorldPosition = GridSlot(nextCoordinates);
    }
    private void UpdateGridReferences()
    {
        for (int i = bodyList.Count - 1; i >= 0; i--)
        {
            if (i == 0)
                GridSnake.instance.PlaceOnGrid(head, nextCoordinates);
            else
            {
                if (bodyList[i].coordinates == bodyList[i - 1].coordinates)
                    continue;

                GridSnake.instance.PlaceOnGrid(bodyList[i],
                                    bodyList[i - 1].coordinates);
            }
        }
    }

    public void Turn(int value)
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


    #endregion

    #region Body Configuration

    public void BodySetup()
    {
        float ratio = 0.4f / bodyList.Count;
        float scale = 1;
        for (int i = 0; i < bodyList.Count; i++)
        {
            bodyList[i].name = "Snake part: " + i;
            bodyList[i].transform.localScale = Vector3.one * scale;
            scale -= ratio;
        }
    }
    public void AddPartInFront(string url)
    {
        SnakeBodyPart p = Instantiate(Resources.Load<SnakeBodyPart>(url));
        p.transform.SetParent(transform);

        Utility.SetColorForAll(p.transform, color);

        p.orientation = head.orientation;
        p.coordinates = head.coordinates;
        p.worldPosition = GridSlot(p.coordinates);
        bodyList.Insert(0, p);


        nextCoordinates = head.coordinates + head.orientation;
        nextWorldPosition = GridSlot(nextCoordinates);

        head.transform.position =
            Vector3.Lerp(head.worldPosition,
                            nextWorldPosition,
                            moveRatio);

    }
    private void AddPartAtEnd()
    {
        SnakeBodyPart p = Instantiate(Resources.Load<SnakeBodyPart>(spareList[0]));

        Utility.SetColorForAll(p.transform, color);

        p.coordinates = tail.coordinates;
        p.orientation = tail.orientation;
        p.worldPosition = tail.worldPosition;

        p.transform.position = tail.worldPosition;
        p.transform.SetParent(transform);

        bodyList.Add(p);

        spareList.RemoveAt(0);

        BodySetup();
    }
    public void RemovePart(int index)
    {
        if (index >= bodyList.Count) return;

        if (index != bodyList.Count - 1)
        {
            for (int i = bodyList.Count - 1; i > index; i--)
            {
                Vector2 coords1 = bodyList[i].coordinates;
                Vector2 coords2 = bodyList[i - 1].coordinates;

                if (coords1 != coords2)
                    GridSnake.instance.PlaceOnGrid(bodyList[i],
                                bodyList[i - 1].coordinates);

                bodyList[i].orientation = bodyList[i - 1].orientation;
                bodyList[i].worldPosition = bodyList[i - 1].worldPosition;
                bodyList[i].transform.position = bodyList[i - 1].transform.position;

            }
        }
        GridSnake.instance.RemoveFromGrid(bodyList[index]);
        Destroy(bodyList[index].gameObject);
        bodyList.RemoveAt(index);
    }

    #endregion

    #region Utility

    public bool IsFromSnake(Entity entity)
    {
        if (entity is SnakeBodyPart)
            return bodyList.Contains((SnakeBodyPart)entity);
        return false;
    }

    public void Eat(Pickable food)
    {
        if (string.IsNullOrEmpty(food.partURL))
        {
            spareList.Add(SnakeBodyPart.PREFAB_URL);
        }
        else if (food == this.food) {
            AddPartInFront(food.partURL);
        }
        else {
            spareList.Add(WrongBodyPart.PREFAB_URL);
         }
    }

    public void UsePower<T>()
    {
        RemovePart(IndexOffType<T>());
        spareList.Add(SnakeBodyPart.PREFAB_URL);
    }

    public void AddSpeedBuff(float buff)
    { speedBuff += buff; }
    public void RemoveSpeedBuff(float buff)
    { speedBuff -= buff; }


    /// <summary>
    /// Auxiliary function: Converts the coordinates to world position
    /// using GridSnake functions.
    /// **GridSnake.instance.getWorldPositionOfSlot(coords);**
    /// </summary>
    private Vector3 GridSlot(Vector2 coords)
    { return GridSnake.instance.GetWorldPositionOfSlot(coords); }


    public SnakeBodyPart FirstOffType<T>()
    {
        foreach (SnakeBodyPart p in bodyList)
        {
            if (p is T) return p;

        }
        return null;
    }
    public int IndexOffType<T>()
    {
        for(int i = 0; i<bodyList.Count; i++)
            if (bodyList[i] is T) return i;

        return -1;
    }

    #endregion
}
