using System.Collections.Generic;
using UnityEngine;

public class BatteringRamPhaseArea : GridSlot
{
    private Snake snake1;
    private Snake snake2;
    private GridSlot oldSlot;

    public override bool hasComflict
    {
        get
        {
            int count = users.Count;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] is SnakeBodyPart)
                {
                    SnakeBodyPart p = (SnakeBodyPart)users[i];
                    if (p.snake == snake1 || p.snake == snake2) count--;
                }
            }
            return count > 1;
        }
    }

    public BatteringRamPhaseArea(GridSlot slot, SnakeBodyPart part1, SnakeBodyPart part2)
        : base(slot.x, slot.y, slot.isBorder)
    {
        snake1 = part1.snake;
        snake2 = part2.snake;


        snake2.AddSpeedBuff(-Snake.WEIGHT_RATIO*3);

        oldSlot = slot;
        oldSlot.ClearUsers();
        GridSnake.instance.ExchangeSlots(this);
        AddUser(part1);
        AddUser(part2);

        snake1.OnDeath += EndPhaseArea;
        snake2.OnDeath += EndPhaseArea;
    }


    public override void RemoveUser(Entity entity)
    {
        if (users.Contains(entity))
        {
            if (entity is SnakeBodyPart)
            {
                SnakeBodyPart bp = (SnakeBodyPart)entity;
                if (bp.tail == bp) EndPhaseArea();
            }
            users.Remove(entity);
            hasChanged = users.Count > 0;
        }
    }

    private void EndPhaseArea()
    {

        snake1.OnDeath -= EndPhaseArea;
        snake2.OnDeath -= EndPhaseArea;
        snake2.AddSpeedBuff(Snake.WEIGHT_RATIO * 3);
        GridSnake.instance.ExchangeSlots(oldSlot);
        //Garbage Collector do your thing
    }


    public override void SolveConflicts(List<SlotActionTemplate> actions)
    {
        /*
        foreach (Entity e in users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart bp = (SnakeBodyPart)e;
                if (bp.snake != snake1 || bp.snake != snake2)
                    e.Contact();
            }
        }*/
        base.SolveConflicts(actions);
    }
}
