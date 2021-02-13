using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;
        if (slot is BatteringRamPhaseArea) return;

        List<SnakeBodyPart> normalBodies = new List<SnakeBodyPart>();
        SnakeBodyPart ramHead = null;

        foreach (Entity e in slot.users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart p = (SnakeBodyPart)e;
                if (p.head == p && p is BatteringRamPart)
                {
                    if (ramHead == null) ramHead = p;
                    else
                    {
                        ramHead.Contact();
                        p.Contact();
                        ramHead = null;
                        
                    }
                }
                else normalBodies.Add(p);
            }
        }
        //no Battering heads
        if (ramHead == null) return;

        //nothing to run over
        if (normalBodies.Count == 0) return;


        ramHead = ramHead.snake.UseHeadPower();

        for (int i = 0; i < normalBodies.Count; i++)
        {
            new BatteringRamPhaseArea(slot, ramHead, normalBodies[i]);
        }

        //ramHead.snake.UsePower<BatteringRamPart>();

    }
}