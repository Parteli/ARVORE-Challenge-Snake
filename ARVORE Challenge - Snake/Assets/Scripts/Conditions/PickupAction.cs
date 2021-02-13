using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAction : SlotActionTemplate
{
    protected static FoodHandler handler
    {
        get { return FoodHandler.instance; }
    }

    protected Pickable food = null;
    protected SnakeBodyPart head = null;

    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

        head = null;
        food = null;

        if (!CheckColision<Pickable>(slot.users.ToArray())) return;


        food.Contact();
        Effect();
    }

    protected virtual bool CheckColision<T>(Entity[] users)
    {
        foreach (Entity e in users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart b = (SnakeBodyPart)e;
                //the first head to arive takes the food
                if (b.head == b && head==null) head = (SnakeBodyPart)e;
            }
            else if (e is T) food = (Pickable)e;
        }

        //needs 1 of each
        return (head != null && food != null);
    }

    protected virtual void Effect()
    {
        //the food is always eaten,
        //the effects change if it belongs to the snake or not
        string foodUrl;
        if (handler.FoodFromPlayer(head.snake, food))
            foodUrl = food.partURL;
        else foodUrl = WrongBodyPart.PREFAB_URL;

        head.snake.Grow(foodUrl);
    }
}
