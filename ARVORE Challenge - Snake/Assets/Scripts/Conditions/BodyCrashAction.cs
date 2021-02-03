using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCrashAction : SlotActionTemplate
{
    public override void Evaluate(GridSlot slot)
    {
        if (!slot.hasComflict) return;

    }
}
