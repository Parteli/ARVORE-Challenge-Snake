using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotActionTemplate: MonoBehaviour
{
    public abstract void Evaluate(GridSlot slot);
}
