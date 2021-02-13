using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnake : MonoBehaviour
{
    #region Provisory Code

    //----------------------------------------------------
    //Create class and interface for map buildning 
    [SerializeField] private Transform tile1;
    [SerializeField] private Transform tile2;
    [SerializeField] private Transform borderTile;
    public int width = 30;
    public int height = 30;

    #endregion

    #region Grid properties
    private static GridSnake _instance = null;
    public static GridSnake instance { 
        get {
            if (_instance == null)
                _instance = FindObjectOfType<GridSnake>(true);

            return _instance;
        }
    }

    private List<List<GridSlot>> slotGrid = null;

    private List<SlotActionTemplate> actionList;
   
    #endregion

    #region Monobehaviour

    public void Setup()
    {
        gameObject.SetActive(true);

        actionList = new List<SlotActionTemplate>();
        actionList.AddRange(GetComponentsInChildren<SlotActionTemplate>(true));


        bool actionBorder = false;
        foreach (SlotActionTemplate act in actionList)
        { if (act is BorderLineAction) actionBorder = true; }
        if (!actionBorder) actionList.Add(gameObject.AddComponent<DeathBorderAction>());


        CreateGrid();
    }

    private void LateUpdate()
    {
        SolveGridConflicts();
    }

    #endregion
  
    #region Grid
    public void CreateGrid()
    {
        Transform holder = (new GameObject()).transform;
        holder.name = "Holder";
        holder.SetParent(transform);

        slotGrid = new List<List<GridSlot>>();

        //positions from 0 to size(width/height)+1
        //the positions 0 and size+1 are the borders and considered death zones
        bool tileType = false;
        bool even = width % 2 == 0;
        for (int y = 0; y <= height+1; y++)
        {
            slotGrid.Add(new List<GridSlot>());
            for (int x = 0; x <= width+1; x++)
            {
                bool b = (x == 0 || y == 0 ||
                    x == width + 1 || y == height + 1);

                slotGrid[y].Add( new GridSlot(x,y,b) );

                Transform tile = null;
                if (b) { tile = Instantiate(borderTile); }
                else
                {
                    tile = Instantiate(tileType ? tile1 : tile2);
                    tileType = !tileType;
                }
                tile.SetParent(holder);
                tile.transform.position = transform.position + new Vector3(x,-0.2f,y);
                tile.name = "Slot("+x+","+y+")";
            }
            if(even) tileType = !tileType;
        }

        Transform camera = FindObjectOfType<Camera>().GetComponent<Transform>();

        float far = 0;
        if (width * 9 >= height * 16) far = width*0.65F;
        else far = height;

        camera.position = transform.position
                    + new Vector3((width+1)*0.5f, far, (height+1)*0.5f);
    }

    public void PlaceOnGrid(Entity entity, Vector2 coordinates)
    {
        GetGridSlot(entity.coordinates).RemoveUser(entity);
        GetGridSlot(coordinates).AddUser(entity);
        entity.coordinates = coordinates;
    }

    public void RemoveFromGrid(Entity entity)
    {
        GetGridSlot(entity.coordinates).RemoveUser(entity);
    }

    public void InitializeAllSnake()
    {
        Snake[] list = GetComponentsInChildren<Snake>();

        Vector2 middle = new Vector2(width*0.5f, height*0.5f);
        Vector2 place = Vector2.one;
        for (int i = 0; i < list.Length; i++)
        {
            Vector2 dif = middle - place;

            if (dif.x >= dif.y)
            {
                dif = new Vector2(Mathf.Sign(dif.x), 0);
            }
            else dif = new Vector2(0, Mathf.Sign(dif.y));

            list[i].StartSetup( place, dif );

            if (place.x + 4 > width)
            {
                place.y += 2;
                place.x += 2 - width;
            }
            else place.x += 4;

        }



    }

    public void InitializeSnake(Snake snake)
    {
        Vector2 coords = RandomEmptyCoordinates();
        Vector2 middle = new Vector2(width * 0.5f, height * 0.5f);
        
        Vector2 dif = middle - coords;

        if (dif.x >= dif.y)
        {
            dif = new Vector2(Mathf.Sign(dif.x), 0);
        }
        else dif = new Vector2(0, Mathf.Sign(dif.y));

        snake.StartSetup(coords, dif);
    }
    public Vector2 RandomEmptyCoordinates()
    {
        Vector2 coords = Vector2.zero;
        do
        {
            coords.x = Mathf.FloorToInt(Random.Range(0, width) + 1);
            coords.y = Mathf.FloorToInt(Random.Range(0, height) + 1);

            //if in use try again
            if (GetGridSlot(coords).isInUse) coords = Vector2.zero;
        }
        while (coords == Vector2.zero);

        return coords;
    }
    #endregion

    #region Action

    private void SolveGridConflicts()
    {
        for (int y = 0; y < slotGrid.Count; y++)
        {
            for (int x = 0; x < slotGrid[y].Count; x++)
            {
                slotGrid[y][x].SolveConflicts(actionList);
            }
        }
    }
    
    #endregion

    #region Utility

    public void ExchangeSlots(GridSlot slot)
    {
        slotGrid[slot.y][slot.x] = slot;
    }
    public bool OutOfBounds(int x, int y)
    { return (x < 0 || y < 0 || x > width + 1 || y > height + 1); }
    public GridSlot GetGridSlot(Vector2 coords)
    {
        return slotGrid[(int)coords.y][(int)coords.x];
    }
    public Vector3 GetWorldPositionOfSlot(int x, int y)
    {
        if (OutOfBounds(x, y)) return Vector3.one * -1;
        return transform.position +
            new Vector3(slotGrid[y][x].x, 0, slotGrid[y][x].y);
    }
    public Vector3 GetWorldPositionOfSlot(Vector2 slot)
    {
        return GetWorldPositionOfSlot((int)slot.x, (int)slot.y);
    }
    #endregion

}
