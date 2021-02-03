using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Snake snake;
    public Snake enemy;
    public Pickable food;

    public KeyCode leftKey;
    public KeyCode rightKey;

    /*
    public bool IsFromPlayer(Snake s1, Snake s2)
    {
        bool check1 = snakeList.Contains(s1) || enemyList.Contains(s1);
        bool check2 = snakeList.Contains(s2) || enemyList.Contains(s2);

        return check1 && check2;
    }
    public bool IsFromPlayer(Snake s1, Pickable p1)
    {
        bool check1 = snakeList.Contains(s1) || enemyList.Contains(s1);
        bool check2 = foodList.Contains(p1);
        return check1 && check2;
    }
    */
}
