using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private static ScoreController _instance = null;
    public static ScoreController instance
    {
        get
        {
            if (_instance == null)
            { _instance = FindObjectOfType<ScoreController>(true); }

            return _instance;
        }
    }


    public RectTransform leftHolder;

    public RectTransform rightHolder;

    public GameObject playerScorePrefab;

    private PlayerScoreDuo[] duoList;

    public void Setup()
    {

        gameObject.SetActive(true);

        List<Player> playerList = GameController.instance.playerList;
        duoList = new PlayerScoreDuo[playerList.Count];
        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerScore ps = Instantiate(Resources.Load<PlayerScore>("Prefabs/Player Score"));
            ps.Setup(playerList[i]);
            ps.transform.SetParent(i%2==0?leftHolder:rightHolder);

            duoList[i] = new PlayerScoreDuo();
            duoList[i].player = playerList[i];
            duoList[i].score = ps;
            duoList[i].player.OnCreateSnake += duoList[i].OnCreateSnake;
        }
    }


    private void OnDestroy()
    {
        foreach (PlayerScoreDuo duo in duoList)
        {
            duo.player.OnCreateSnake -= duo.OnCreateSnake;
        }
    }

    internal class PlayerScoreDuo
    {
        public Player player;
        public PlayerScore score;

        public void OnMoveGrid()
        {
            score.AddScore(player.snake.bodyList.Count);
        }
        public void OnDeath()
        {
            score.score *= Mathf.FloorToInt(0.5f);
            score.AddScore(0);

            player.snake.OnMoveGrid -= OnMoveGrid;
            player.snake.OnDeath -= OnDeath;
            player.snake.OnEat -= OnEat;
        }
        public void OnEat(bool value)
        {
            score.AddScore(value ? 500 : 100);
        }

        public void OnCreateSnake(Snake snake)
        {
            snake.OnMoveGrid += OnMoveGrid;
            snake.OnDeath += OnDeath;
            snake.OnEat += OnEat;
        }

    }
}
