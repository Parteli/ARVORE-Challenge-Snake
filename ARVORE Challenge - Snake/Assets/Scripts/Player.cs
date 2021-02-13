using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public PlayerTag playerTag;

    public PlayerControlSnake snakeController;
    public Snake snake
    {
        get
        {
            if (snakeController == null) return null;
            return snakeController.snake;
        }
    }
    public bool isSnakeActive { get { return !(snake == null || snake.isDead); } }

    private NpcControlSnake npcSnake;
    public Snake enemy
    {
        get {
            if (npcSnake == null) return null;

            return npcSnake.snake;
        }
    }

    /*
    private Pickable _food;
    public Pickable food
    {
        get { return _food; }

        set {
            if (_food != null) GameObject.Destroy(_food);
            _food = value;
            snake.food = value;

            if (enemy != null) enemy.food = value;

            Utility.SetColorForAll(_food.transform, color);
        }
    }
    */
    public KeyCode leftKey;
    public KeyCode rightKey;

    public Color color;

    public List<string> snakeData = null;

    public event Utility.GetSnakeDelegate OnCreateSnake = delegate {};

    public void StartMatch()
    {
        CreatePlayerSnake();
        CreateEnemySnake();
    }

    public void CreatePlayerSnake()
    {
        if (snake != null)
        {
            snake.gameObject.SetActive(false);
            GameObject.Destroy(snake.gameObject);
            snakeController = null;
        }

        snakeController = (new GameObject()).AddComponent<PlayerControlSnake>();
        snakeController.transform.SetParent(GridSnake.instance.transform);
        snakeController.player = this;

        Color c = color;
        c.a = 0.7f;
        snakeController.snake.color = c;
        snakeController.snake.name = "Snake: " +
            Utility.LastLetterOf(leftKey.ToString()) +
            Utility.LastLetterOf(rightKey.ToString());


        snakeController.snake.spareList = new List<string>();
        snakeController.snake.spareList.AddRange(snakeData);

        //if (food != null) snakeController.snake.food = food;

        OnCreateSnake(snakeController.snake);
    }


    //Create a Controller to handle the enemy snakes 
    public void CreateEnemySnake()
    {
        if (enemy != null)
        {
            FoodHandler.instance.RemoveMessageFromPlayerFood(this, npcSnake.SetTargetFood);
            enemy.gameObject.SetActive(false);
            GameObject.Destroy(enemy.gameObject);
            npcSnake = null;
        }

        npcSnake = (new GameObject()).AddComponent<NpcControlSnake>();
        npcSnake.transform.SetParent(GridSnake.instance.transform);

        npcSnake.snake.color = color;
        npcSnake.snake.name = "Enemy: " +
            Utility.LastLetterOf(leftKey.ToString()) +
            Utility.LastLetterOf(rightKey.ToString());


        npcSnake.snake.spareList = new List<string>();
        npcSnake.snake.spareList.Add(SnakeBodyPart.PREFAB_URL);
        npcSnake.snake.spareList.Add(SnakeBodyPart.PREFAB_URL);


        FoodHandler.instance.AddMessageToPlayerFood(this, npcSnake.SetTargetFood);
        //if (food != null) npcSnake.snake.food = food;
    }
}
