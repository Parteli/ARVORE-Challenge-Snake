using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnake : MonoBehaviour
{
    private static GridSnake _instance = null;
    public static GridSnake instance { 
        get {
            if (_instance == null)
                _instance = FindObjectOfType<GridSnake>();

            return _instance;
        }
    }

    [SerializeField] private Transform tile1;
    [SerializeField] private Transform tile2;
    [SerializeField] private Transform borderTile;

    public int width = 50;
    public int height = 50;

    private List<List<GridSlot>> slotGrid = null;


    private void Awake()
    {
        CreateGrid();
    }

    //------------------------------------------------------
    //Create class for item creation and distribution
    private Pickable food = null;
    public void Update()
    {
        if (food == null)
        {
            if (Random.value > 0.5f)
                food = Instantiate(Resources.Load<Pickable>("Prefabs/Pickable_Food"));
            else food = Instantiate(Resources.Load<Pickable>("Prefabs/BatteringRamFood"));

            Vector2 coords = Vector2.zero;
            do
            {
                coords.x = Mathf.FloorToInt(Random.Range(0, width) + 1);
                coords.y = Mathf.FloorToInt(Random.Range(0, height) + 1);

                //if in use try again
                if( getGridSlot(coords).isInUse ) coords = Vector2.zero;
            } 
            while (coords == Vector2.zero);

            PlaceOnGrid(food, coords);
            food.transform.position = getWorldPositionOfSlot(coords);
        }
    }
    //--------------------------------------------------

    public void CreateGrid()
    {
        slotGrid = new List<List<GridSlot>>();

        //positions from 0 to size(width/height)+1
        //the positions 0 and size+1 are the borders and considered death zones
        bool tileType = false;
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
                tile.SetParent(transform);
                tile.transform.position = transform.position + new Vector3(x,-0.2f,y);
                tile.name = "Slot("+x+","+y+")";
            }
            tileType = !tileType;
        }

        Transform camera = FindObjectOfType<Camera>().GetComponent<Transform>();

        camera.position = transform.position
                    + new Vector3(width*0.5f, height, height*0.5f);
    }

    public Vector3 getWorldPositionOfSlot(int x, int y)
    {
        if (OutOfBounds(x,y)) return Vector3.one*-1;
        return transform.position +
            new Vector3(slotGrid[y][x].x, 0, slotGrid[y][x].y);
    }
    public Vector3 getWorldPositionOfSlot(Vector2 slot)
    {
        return getWorldPositionOfSlot((int)slot.x, (int)slot.y);
    }
    private Vector3 getWorldPositionOfSlot(GridSlot slot)
    {
        return getWorldPositionOfSlot(slot.x, slot.y);
    }

    public void PlaceOnGrid(Entity entity, Vector2 coordinates)
    {
        getGridSlot(entity.coordinates).RemoveUser(entity);
        getGridSlot(coordinates).AddUser(entity);
        entity.coordinates = coordinates;
    }

    public void RemoveFromGrid(Entity entity)
    {
        getGridSlot(entity.coordinates).RemoveUser(entity);
    }

    public GridSlot getGridSlot(Vector2 coords)
    {
        return slotGrid[(int)coords.y][(int)coords.x];
    }
    public bool OutOfBounds(int x, int y)
    { return (x < 0 || y < 0 || x > width + 1 || y > height + 1); }
    
    public List<GridSlot> FindConflicts()
    {
        List<GridSlot> cases = new List<GridSlot>();
        foreach (List<GridSlot> line in slotGrid)
        {   
            foreach (GridSlot slot in line)
            {   
                if( (slot.isBorder && slot.isInUse) ||
                    (slot.hasChanged && slot.hasComflict) )
                        cases.Add(slot);
            }
        }

        return cases;
    }

    public void ExchangeSlots(GridSlot slot)
    {
        slotGrid[slot.y][slot.x] = slot;
    }
}
