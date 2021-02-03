using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyPart : Entity
{
    public static string PREFAB_URL = "Prefabs/BodyPart_Normal"; 

    [HideInInspector]
    public Vector3 worldPosition;

    private Snake _snake;
    public Snake snake
    {
        get
        {
            if (_snake == null)
                _snake = GetComponentInParent<Snake>();
            return _snake;
        } 
    }

    public SnakeBodyPart getHead { get { return snake.head; } }
    public SnakeBodyPart getTail { get {return snake.tail; } }

    public override void Contact()
    {
        snake.Death();
    }
}
