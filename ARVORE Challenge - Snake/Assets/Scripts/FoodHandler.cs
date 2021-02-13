using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHandler : MonoBehaviour
{
    #region References
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

    private GameController game { get { return GameController.instance; } }
    public static GridSnake grid { get { return GridSnake.instance; } }

    [SerializeField]
    private List<Pickable> foodPrefabList;

    [SerializeField]
    private List<Pickable> fruitPrefabList;
    public Pickable currentFruit { get; private set; }

    private PlayerFoodDuo[] duoList;
    #endregion

    #region Timer properties

    [SerializeField]
    private float timer = 10;
    private float timerCount = 0;

    [SerializeField]
    private Vector2 playerFoodOverrideTimer = new Vector2(10, 20);
    [SerializeField]
    private float playerFoodCooldownTimer = 3;

    #endregion

    #region Monobehaviour
    public void Setup()
    {
        gameObject.SetActive(true);
        List<Player> playerList = game.playerList;
        duoList = new PlayerFoodDuo[playerList.Count];
        for (int i = 0; i < playerList.Count; i++)
        {
            duoList[i] = new PlayerFoodDuo();
            duoList[i].player = playerList[i];
            duoList[i].player.OnCreateSnake += OnCreateSnake;
            duoList[i].timerCount = Time.timeSinceLevelLoad + playerFoodCooldownTimer;
        }
    }
    private void OnDestroy()
    {
        foreach (PlayerFoodDuo duo in duoList)
            duo.player.OnCreateSnake -= OnCreateSnake;
    }

    private void Update()
    {

        if (Time.timeSinceLevelLoad - timerCount > timer)
        { CreateFruit(); }

        foreach (PlayerFoodDuo duo in duoList)
        {
            if (!duo.player.isSnakeActive) return;
            if (Time.timeSinceLevelLoad > duo.timerCount)
            { CreatePlayerFood(duo); }
        }
    }

    #endregion
    private void CreateFruit()
    {
        if (currentFruit != null)
        {
            grid.RemoveFromGrid(currentFruit);
            Destroy(currentFruit.gameObject);
            currentFruit.gameObject.SetActive(false);
            currentFruit = null;
        }

        Pickable fruit = Instantiate(RandomFood(fruitPrefabList));
        fruit.transform.SetParent(transform);
        currentFruit = fruit;
        RandomPlacement(fruit);

        timerCount = Time.timeSinceLevelLoad;
    }


    private void OnCreateSnake(Snake snake)
    {
        snake.OnEat += SnakeEats;
    }

    private void SnakeEats(bool value)
    {
        foreach (PlayerFoodDuo duo in duoList)
        {
            if (!duo.player.isSnakeActive) continue;
            if (duo.food == null || !duo.food.isActiveAndEnabled) 
                duo.timerCount = Time.timeSinceLevelLoad+ playerFoodCooldownTimer;
        }
    }

    private void CreatePlayerFood(PlayerFoodDuo duo)
    {
        Pickable food = Instantiate(RandomFood(foodPrefabList));
        food.transform.SetParent(transform);
        duo.food = food;
        RandomPlacement(food);

        duo.timerCount = Time.timeSinceLevelLoad +
            Random.Range(playerFoodOverrideTimer.x, playerFoodOverrideTimer.y);
    }

    private Pickable RandomFood(List<Pickable> list)
    {
        int value = 0;

        value = Random.Range(0, list.Count);

        return list[value];
    }

    private void RandomPlacement(Pickable food)
    {
        Vector2 coords = grid.RandomEmptyCoordinates();

        grid.PlaceOnGrid(food, coords);
        food.transform.position = grid.GetWorldPositionOfSlot(coords);
    }


    public void AddMessageToPlayerFood(Player player, Utility.GetPickableDelegate action)
    {
        foreach (PlayerFoodDuo duo in duoList)
        {
            if (player == duo.player)
            {
                duo.PlaceNewFood += action;
            }
        }
    }
    public void RemoveMessageFromPlayerFood(Player player, Utility.GetPickableDelegate action)
    {
        foreach (PlayerFoodDuo duo in duoList)
        {
            if (player == duo.player)
            {
                duo.PlaceNewFood -= action;
            }
        }
    }

    public bool FoodFromPlayer(Snake snake, Pickable food)
    {
        foreach (PlayerFoodDuo duo in duoList)
        {
            if (duo.player.snake == snake || duo.player.enemy == snake)
            {
                if (duo.food == food) return true;
            }
            else if (duo.food == food) return false;
        }

        return true;
    }

    internal class PlayerFoodDuo
    {
        public Player player;
        private Pickable _food;
        public float timerCount;

        public event Utility.GetPickableDelegate PlaceNewFood = delegate { };

        public Pickable food
        {
            get { return _food; }

            set
            {
                if (_food != null) _food.Contact();

                _food = value;
                Utility.SetColorForAll(_food.transform, player.color);
                PlaceNewFood(_food);
            }
        }

    }
}

