using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelFood : Pickable
{
    public override string partURL
    {
        get { return TimeTravelPart.PREFAB_URL; }
    }
}
