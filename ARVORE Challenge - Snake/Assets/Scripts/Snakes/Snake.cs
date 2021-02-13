using System;
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
    public const float MAX_SPEED = 20;
    public const float MIN_SPEED = 2;
    public const float BASE_SPEED = 10;
    public const float WEIGHT_RATIO = 0.5f;

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
    [HideInInspector]
    public List<SnakeBodyPart> bodyList = new List<SnakeBodyPart>();
    //stores the initial setup and later is used to add parts at the tail
    [HideInInspector]
    public List<string> spareList = new List<string>();
    public SnakeBodyPart head { get { return bodyList[0]; } }
    public SnakeBodyPart tail { get { return bodyList[bodyList.Count - 1]; } }

    //public Pickable food;

    [HideInInspector] public Color color;

    [HideInInspector] public bool isDead = false;


    public event Action OnMoveGrid = delegate { };
    public event Action OnDeath = delegate { };
    public event Utility.GetBoolDelegate OnEat = delegate { };

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
            p.coordinates = coordinates;

            Utility.SetColorForAll(p.transform, color);
        }

        CheckHeadPower(false);

        GridSnake.instance.PlaceOnGrid(head, coordinates);

        spareList.Clear();

        nextDirection = direction;
        nextCoordinates = coordinates + direction;
        nextWorldPosition = GridSlot(nextCoordinates);

        BodyScale();
    }

    public void Death()
    {
        if (isDead) return;
        OnDeath?.Invoke();
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
        OnMoveGrid?.Invoke();
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

    public void BodyScale()
    {
        float ratio = 0.4f / bodyList.Count;
        float scale = 1;
        for (int i = 0; i < bodyList.Count; i++)
        {
            bodyList[i].name = "Snake part: " + i + " | " + bodyList[i].GetType().ToString();
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

        CheckHeadPower(true);

        BodyScale();
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

        BodyScale();
    }
    public void RemovePart(int index)
    {
        if (index >= bodyList.Count) return;

        if (index == 0)
        {
            int i = IndexOffType<HeadPowerPart>(0);
            if (i > 0)
            {
                SwitchParts(index, i);
                index = i;
            }
        }

        MovePart(index, bodyList.Count-1);
        index = bodyList.Count - 1;

        GridSnake.instance.RemoveFromGrid(bodyList[index]);
        Destroy(bodyList[index].gameObject);
        bodyList.RemoveAt(index);
    }

    /// <summary>
    /// Select 2 parts and exchange their places.
    /// This will not affect the rest of the snake.
    /// </summary>
    /// <param name="first">The index of the first part.</param>
    /// <param name="second">The index of the second part.</param>
    public void SwitchParts(int first, int second, bool placeOnGrid = true)
    {
        BodyPartInfo info1 = new BodyPartInfo(bodyList[first]);
        BodyPartInfo info2 = new BodyPartInfo(bodyList[second]);

        SnakeBodyPart p1 = bodyList[first];
        SnakeBodyPart p2 = bodyList[second];
        bodyList[second] = p1;
        bodyList[first] = p2;

        info1.Apply(p2, placeOnGrid);
        info2.Apply(p1, placeOnGrid);

    }
    /// <summary>
    /// Move the selected part to a defined position.
    /// This will move all the parts between the original position
    /// and its destination forward or backwards depending on where
    /// the the part moved was initially. 
    /// </summary>
    /// <param name="index">The index of the part do move.</param>
    /// <param name="to">The position to where the part should go.</param>
    public void MovePart(int index, int to, bool placeOnGrid = true)
    {
        if (index == to) return;


        BodyPartInfo[] infoList = new BodyPartInfo[bodyList.Count];

        for (int i = 0; i < bodyList.Count; i++)
        {
            infoList[i] = new BodyPartInfo(bodyList[i]);
        }

        SnakeBodyPart p = bodyList[index];
        if (index < to)
        {
            for (int i = index + 1; i <= to; i++)
            {
                infoList[i - 1].Apply(bodyList[i], placeOnGrid);
                bodyList[i - 1] = bodyList[i];
            }
        }
        else
        {
            for (int i = index - 1; i >= to; i--)
            {
                infoList[i + 1].Apply(bodyList[i], placeOnGrid);
                bodyList[i + 1] = bodyList[i];
            }
        }
        infoList[to].Apply(p, placeOnGrid);
        bodyList[to] = p;
    }

    public SnakeBodyPart SwapParts(SnakeBodyPart part, int index, bool destroy = false)
    {
        BodyPartInfo info = new BodyPartInfo(bodyList[index]);
        SnakeBodyPart oldPart = bodyList[index];
        GridSnake.instance.RemoveFromGrid(oldPart);

        info.Apply(part);
        bodyList[index] = part;

        Utility.SetColorForAll(part.transform, color);
        part.transform.SetParent(transform);
        GridSnake.instance.PlaceOnGrid(part, part.coordinates);

        if (destroy)
        {
            Destroy(oldPart.gameObject);
            return null;
        }
        return oldPart;
    }

    public void Grow(string prefab)
    {
        if (string.IsNullOrEmpty(prefab)) { spareList.Add(SnakeBodyPart.PREFAB_URL); }
        else if(prefab.Equals(SnakeBodyPart.PREFAB_URL) || prefab.Equals(WrongBodyPart.PREFAB_URL))
        {
            spareList.Add(prefab);
        }
        else
        {
            AddPartInFront(prefab);
        }
        OnEat?.Invoke(true);
    }

    /*
    public virtual string SaveInfo(string info = "")
    {
        info += "&c," + nextCoordinates.x + "," + nextCoordinates.y;
        info += "&o," + nextDirection.x + "," + nextDirection.y;
        info += "&s," + bodyList.Count;
        info += "&p";

        for (int i = 0; i < bodyList.Count; i++)
        {
            info += ",";
            info += bodyList[i].SaveInfo();
        }

        info += "&t," + GetType().ToString();
        return info;

        
    }
    public virtual string[][] LoadInfo(string info)
    {
        string[] split1 = info.Split("&"[0]);

        string[][] r = new string[split1.Length][];

        bodyList = new List<SnakeBodyPart>();
        int count = 0;

        for (int i = 0; i < split1.Length; i++)
        {
            string[] split2 = split1[i].Split(","[0]);
            r[i] = new string[split2.Length];
            for (int j = 0; j < split2.Length; j++)
            {
                r[i][j] = split2[j];
                switch (split2[0])
                {
                    case "c":
                        nextCoordinates.x = int.Parse(split2[1]);
                        nextCoordinates.y = int.Parse(split2[2]);
                        break;
                    case "o":
                        nextDirection.x = int.Parse(split2[1]);
                        nextDirection.y = int.Parse(split2[2]);
                        break;
                    case "s": count = int.Parse(split2[1]);
                        break;
                    case "p":
                        for (int k = 1; k <= count; k++)
                        {
                           // string[] split3 = split2[k].Split("#"[0]);
                           // System.Type type = System.Type.GetType(split3[split3.Length-1]);
                           // string url = ((SnakeBodyPart)type).prefab_url; 
                            
                        }
                        break;
                }
            }
        }


        return r;
    }

    private void LoadBodyInfo(string[][] info)
    { 
    
    }
    */
    #endregion

    #region Utility

    private void CheckHeadPower(bool placeOnGrid)
    {
        if (!(head is HeadPowerPart))
        {
            int count = 0;
            while (count >= 0)
            {
                int nextHeadPower = IndexOffType<HeadPowerPart>(count);
                if (nextHeadPower > count)
                {
                    MovePart(nextHeadPower, count, placeOnGrid);
                    count++;
                }
                else count = -1;
            }
        }
    }

    /*
    public bool IsFromSnake(Entity entity)
    {
        if (entity is SnakeBodyPart)
            return bodyList.Contains((SnakeBodyPart)entity);
        return false;
    }
    public bool Eat(Pickable food)
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
            OnEat?.Invoke(false);
            return false;
        }
        OnEat?.Invoke(true);
        return true;
    }
    */
   

    public void UsePower<T>()
    {
        RemovePart(IndexOffType<T>());
        spareList.Add(SnakeBodyPart.PREFAB_URL);
    }

    public SnakeBodyPart UseHeadPower()
    {
        SnakeBodyPart part =
            Instantiate(Resources.Load<SnakeBodyPart>(SnakeBodyPart.PREFAB_URL));

        SwapParts(part, 0, true);
        CheckHeadPower(true);
        BodyScale();

        return part;
    }

    public void AddSpeedBuff(float buff)
    { speedBuff += buff; }


    /// <summary>
    /// Auxiliary function: Converts the coordinates to world position
    /// using GridSnake functions.
    /// **GridSnake.instance.getWorldPositionOfSlot(coords);**
    /// </summary>
    private Vector3 GridSlot(Vector2 coords)
    { return GridSnake.instance.GetWorldPositionOfSlot(coords); }

    public bool IsFirstPower<T>()
    {
        foreach (SnakeBodyPart p in bodyList)
        {
            if (p is T) return true;

        }
        return false;
    }

    public SnakeBodyPart FirstOffType<T>()
    {
        foreach (SnakeBodyPart p in bodyList)
        {
            if (p is T) return p;

        }
        return null;
    }
    public int IndexOffType<T>(int index = -1)
    {
        if (index < -1 || index >= bodyList.Count) index = -1;

        for(int i = index +1; i<bodyList.Count; i++)
            if (bodyList[i] is T) return i;

        return -1;
    }

    #endregion
}

internal class BodyPartInfo
{
    private Vector2 orientation;
    private Vector2 coordinates;
    private Vector3 worldPosition;
    private Vector3 currentPosition;

    public BodyPartInfo(SnakeBodyPart part)
    {
        orientation = part.orientation;
        coordinates = part.coordinates;
        worldPosition = part.worldPosition;
        currentPosition = part.transform.position;
    }

    public void Apply(SnakeBodyPart part, bool placeOnGrid = true)
    {
        part.orientation = orientation;
        part.worldPosition = worldPosition;
        part.transform.position = currentPosition;
        if (placeOnGrid) GridSnake.instance.PlaceOnGrid(part, coordinates);
        else part.coordinates = coordinates;
    }
}
