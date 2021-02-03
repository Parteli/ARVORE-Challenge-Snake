using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

        List<BatteringRamPart> ramHeads = new List<BatteringRamPart>();
        List<SnakeBodyPart> normalBodies = new List<SnakeBodyPart>();

        foreach (Entity e in slot.users)
        {
            if (e is BatteringRamPart)
            {
                BatteringRamPart b = (BatteringRamPart)e;
                //only works when Battering Ram is the head
                if (b.getHead == b) ramHeads.Add((BatteringRamPart)e);
            }
            else if (e is SnakeBodyPart) normalBodies.Add((SnakeBodyPart)e);
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

        ramHeads[ramHeads.Count - 1].snake.UseHeadPower();
      
    }
}