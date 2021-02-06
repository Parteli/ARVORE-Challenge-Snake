using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _instance = null;
    public static GameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>(true);
                if (_instance == null)
                {
                    _instance = (new GameObject()).AddComponent<GameController>();
                    _instance.name = "__GameController__";
                }
            }

            return _instance;
        }
    }

    private List<Player> _playerList;
    public List<Player> playerList 
    {
        get
        {
            if (_playerList == null)
            { _playerList = new List<Player>(); }
            return _playerList;
        }
    }

    private bool onMatch = false;



    public void StartMatch()
    {
        onMatch = true;

        GridSnake.instance.gameObject.SetActive(true);
        FoodHandler.instance.gameObject.SetActive(true);

        foreach (Player p in playerList) p.StartMatch();

        GridSnake.instance.InitializeAllSnake();

    }

    private void LateUpdate()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) )
        {
            if (onMatch)
            {
                int scene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
            else Application.Quit();
        }

        if (onMatch) Respawn();
    }

    private void Respawn()
    {
        foreach (Player p in playerList)
        {
            if (p.snake.isDead)
            {
                if (Input.GetKeyDown(p.rightKey) && Input.GetKeyDown(p.leftKey))
                {
                    p.CreatePlayerSnake();
                    GridSnake.instance.InitializeSnake(p.snake);
                }
            }
            else if (p.enemy.isDead)
            {
                p.CreateEnemySnake();
                GridSnake.instance.InitializeSnake(p.enemy);
            }
        }
    }
}













