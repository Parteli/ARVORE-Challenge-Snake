using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Snake))]
public abstract class SnakeController : MonoBehaviour
{
    private Snake _snake;
    public Snake snake 
    {
        get {
            if (_snake == null) _snake = GetComponent<Snake>();
            return _snake;
        }
    }

    private void Update()
    {
        if (snake.isDead) return;

        DirectionManager();
        snake.CustomUpdate();

    }

    protected abstract void DirectionManager();

}
