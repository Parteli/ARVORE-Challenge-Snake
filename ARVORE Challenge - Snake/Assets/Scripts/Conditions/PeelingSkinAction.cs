using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelingSkinAction : PickupAction
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

        head = null;
        food = null;

        if (!CheckColision<PeelingSkinFruit>(slot.users.ToArray())) return;

        food.Contact();
        Effect();

    }

    protected override void Effect()
    {
        //the food is always eaten,
        //the effects change if it belongs to the snake or not
        string foodUrl;
        if (handler.FoodFromPlayer(head.snake, food))
        {
            Snake sn = head.snake;
            for (int i = 0; i < sn.bodyList.Count; i++)
            {
                if (sn.bodyList[i] is WrongBodyPart)
                {
                    SnakeBodyPart goodPart =
                        Instantiate(Resources.Load<SnakeBodyPart>(SnakeBodyPart.PREFAB_URL));

                    sn.SwapParts(goodPart, i, true);
                    sn.AddSpeedBuff(Snake.WEIGHT_RATIO);
                }
            }
            sn.BodyScale();
            foodUrl = SnakeBodyPart.PREFAB_URL;
        }
        else foodUrl = WrongBodyPart.PREFAB_URL;

        head.snake.Grow(foodUrl);
    }
}
