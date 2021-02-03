using System.Collections.Generic;
using UnityEngine;

public class BatteringRamPhaseArea : GridSlot
{
    private Snake snake1;
    private Snake snake2;
    private GridSlot oldSlot;
  
    public override bool hasChanged
    {
        get { return oldSlot.hasChanged; }
        set { oldSlot.hasChanged = value; }
    }

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

    public BatteringRamPhaseArea(GridSlot slot, Snake snake1, Snake snake2)
        : base(slot.x, slot.y, slot.isBorder)
    {
        this.snake1 = snake1;
        this.snake2 = snake2;

        Debug.LogWarning("Second snake must be slowed and have its score frozen");

        oldSlot = slot;
        oldSlot.ClearUsers();
        GridSnake.instance.ExchangeSlots(this);
    }


    public override void RemoveUser(Entity entity)
    {
        if (users.Contains(entity))
        {
            if (entity is SnakeBodyPart)
            {
                SnakeBodyPart bp = (SnakeBodyPart)entity;
                if (bp.getTail == bp) EndPhaseArea();
            }
            users.Remove(entity);
            hasChanged = users.Count > 0;
        }
    }

    private void EndPhaseArea()
    {
        GridSnake.instance.ExchangeSlots(oldSlot);
        //Garbage Collector do your thing
    }
    
}
