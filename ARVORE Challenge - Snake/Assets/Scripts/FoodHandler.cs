using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHandler : MonoBehaviour
{
    private static FoodHandler _instance = null;
    public static FoodHandler instance
    {
        get
        {
            if (_instance == null)
            { _instance = FindObjectOfType<FoodHandler>(true); }

            return _instance;
        }
    }

    private GameController gameController;

    private static GridSnake _grid;
    public static GridSnake grid
    {
        get
        {
            if (_grid == null)
            { _grid = GridSnake.instance; }

            return _grid;
        }
    }

    [SerializeField]
    private List<Pickable> foodPrefabList;

    private List<Player> playerList
    {
        get
        {
            if (gameController == null) gameController = GameController.instance;
            return gameController.playerList;
        }
    }



    private void Update()
    {
        //use delegates and messages to control when food are created or destroyed
        foreach (Player p in playerList)
        {
            if (!p.isSnakeActive) continue;

            if (p.food == null)
            {
                Pickable food = Instantiate(RandomFood());
                food.transform.SetParent(transform);
                p.food = food;
                RandomPlacement(food);
            }
        }
    }

    private Pickable RandomFood()
    {
        int value = 0;

        value = Random.Range(0,foodPrefabList.Count);

        return foodPrefabList[value];
    }

    private void RandomPlacement(Pickable food)
    {
        Vector2 coords = grid.RandomEmptyCoordinates();

        grid.PlaceOnGrid(food, coords);
        food.transform.position = grid.GetWorldPositionOfSlot(coords);
    }

}
