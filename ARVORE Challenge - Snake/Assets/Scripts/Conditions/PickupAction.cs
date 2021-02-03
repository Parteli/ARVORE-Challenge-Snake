using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

        Pickable food = null;
        SnakeBodyPart head = null;

        foreach (Entity e in slot.users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart b = (SnakeBodyPart)e;
                //the first head to arive takes the food
                if (b.getHead == b) head = (SnakeBodyPart)e;
            }
            else if (e is Pickable) food = (Pickable)e;
        }

        //needs 1 of each
        if (head == null || food == null) return;

        //the food is always eaten,
        //the effects change if it belongs to the snake or not
        head.snake.Eat(food);
        head.snake.BodySetup();

        food.Contact();
        slot.RemoveUser(food);
    }
}
