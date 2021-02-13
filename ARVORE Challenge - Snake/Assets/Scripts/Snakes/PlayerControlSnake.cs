using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlSnake : SnakeController
{
    private Player _player;
    public Player player 
    {
        get { return _player; }
        set
        {
            if (_player == null) _player = value;
        }
    }

    public KeyCode left;
    public KeyCode right;


    public List<string> dataList;
    
    private void Start()
    {
        if (!GameController.instance.debugMode) return;
        player = new Player();
        player.rightKey = right;
        player.leftKey = left;
        player.snakeController = this;

        GameController.instance.playerList.Add(player);

        snake.color = player.color = Random.ColorHSV();
        snake.spareList = dataList;
        snake.StartSetup(snake.nextCoordinates, snake.nextDirection);
    }
    

    protected override void DirectionManager()
    {
        if (Input.GetKeyDown(player.leftKey)) snake.Turn(-1);
        else if (Input.GetKeyDown(player.rightKey)) snake.Turn(1);
    }

}
