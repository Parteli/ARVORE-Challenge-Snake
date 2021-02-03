using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCrashAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;


        List<SnakeBodyPart> heads = new List<SnakeBodyPart>();
        int bodyCount = 0;
        foreach (Entity e in slot.users)
        {
            if (e is SnakeBodyPart)
            {
                SnakeBodyPart b = (SnakeBodyPart)e;
                
                //separating bodies and heads
                if (b.getHead == b) heads.Add(b);
                else bodyCount++;
            }
        }

        //no colisions happening
        if ( (heads.Count + bodyCount) <= 1) return;

        //all heads die
        foreach (SnakeBodyPart h in heads) h.Contact();
    }
}
