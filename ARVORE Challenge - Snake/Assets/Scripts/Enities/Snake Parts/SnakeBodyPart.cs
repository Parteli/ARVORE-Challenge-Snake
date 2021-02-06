using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyPart : Entity
{
    public virtual string prefab_url { get { return PREFAB_URL; } }
    public static string PREFAB_URL = "Prefabs/BodyPart_Normal";
    public virtual string icon_url { get { return ICON_URL; } }
    public static string ICON_URL = "Icons/heart";
    public virtual string small_text { get { return SMALL_TEXT; } }
    public static string SMALL_TEXT = "A normal snake";
    public virtual string description { get { return DESCRIPTION; } }
    public static string DESCRIPTION = "Just a normal snake. Has no special effects.";


    public override Vector2 orientation
    { get => base.orientation;
        set
        {
            _orientation = value;
            Vector3 v = new Vector3(value.x,0,value.y);
            transform.LookAt(transform.position + v);
        }
    }

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


    public SnakeBodyPart head { get { return snake.head; } }
    public SnakeBodyPart tail { get {return snake.tail; } }

    public override void Contact()
    {
        snake.Death();
    }
}
