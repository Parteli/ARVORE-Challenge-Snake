using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RottenFruitAction : PickupAction
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

        head = null;
        food = null;

        if (!CheckColision<RottenFruit>(slot.users.ToArray())) return;

        Effect();

        food.Contact();
    }

    protected override void Effect()
    {
        Snake sn = head.snake;
        for (int i = 0; i < sn.bodyList.Count; i++)
        {
            if (!(sn.bodyList[i] is WrongBodyPart))
            {
                SnakeBodyPart badPart =
                    Instantiate(Resources.Load<SnakeBodyPart>(WrongBodyPart.PREFAB_URL));

                sn.SwapParts(badPart, i, true);
            }
        }
        sn.BodyScale();

        head.snake.Grow(WrongBodyPart.PREFAB_URL);
    }
}
