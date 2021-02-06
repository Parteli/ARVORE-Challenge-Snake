using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;
        List<SnakeBodyPart> ramHeads = new List<SnakeBodyPart>();
        List<SnakeBodyPart> normalBodies = new List<SnakeBodyPart>();

        foreach (Entity e in slot.users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart p = (SnakeBodyPart)e;
                if (p.head == p)
                {
                    if (p.snake.FirstOffType<BatteringRamPart>() != null)
                    {
                        ramHeads.Add(p);
                        continue;
                    }
                }
                normalBodies.Add(p);
            }
        }
        //no Battering heads
        if (ramHeads.Count == 0) return;

        //nothing to run over
        if (ramHeads.Count + normalBodies.Count <= 1) return;

        //if more than 1 battering head
        //the last head to arrive run over the rest
        for (int i = 0; i < ramHeads.Count - 1; i++)
        {
            new BatteringRamPhaseArea(slot,
                    ramHeads[ramHeads.Count - 1].snake,
                    ramHeads[i].snake);
        }
        //normal bodies always get trampled
        for (int i = 0; i < normalBodies.Count; i++)
        {
            new BatteringRamPhaseArea(slot,
                    ramHeads[ramHeads.Count - 1].snake,
                    normalBodies[i].snake);
        }

        ramHeads[ramHeads.Count - 1].snake.UsePower<BatteringRamPart>();
      
    }
}