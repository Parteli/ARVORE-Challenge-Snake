using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcControlSnake : SnakeController
{
    public int awareness = 5;
    public float reactionTime = 0.5f;

    private float timer = 0;
    private GridSnake grid
    {
        get
        {
            return GridSnake.instance;
        } 
    }

    protected override void DirectionManager()
    {
        
        if (Time.timeSinceLevelLoad - timer < reactionTime) return;

        if (snake.food == null) AvoidBehaviour();
        else FarFoodBehaviour();

        timer = Time.timeSinceLevelLoad;
    }

    private void FarFoodBehaviour()
    {
        Vector2 dif = snake.food.coordinates - snake.nextCoordinates;

        Vector2 direction = Vector2.zero;
        if (Mathf.Abs(dif.x) >= Mathf.Abs(dif.y))
            direction.x = Mathf.Sign(dif.x);
        else direction.y = Mathf.Sign(dif.y);

        //horizontal
        if (snake.head.orientation.x != 0)
        {
            // 90º
            if (direction.x == 0)
            {
                snake.Turn((int)-(snake.head.orientation.x*direction.y));
            }
            //wrong way 180º
            else if (direction.x != snake.head.orientation.x)
            {
                snake.Turn((int)(Mathf.Sign(dif.y) * direction.x));
            }
        }
        //vertical
        else
        {
            // 90º
            if (direction.y == 0)
            {
                snake.Turn((int)(snake.head.orientation.y * direction.x));
            }
            //wrong way 180º
            else if (direction.y != snake.head.orientation.y)
            {
                snake.Turn((int)-(Mathf.Sign(dif.x) * direction.y));
            }
        }
    }

    private void AvoidBehaviour()
    {

    }
}
