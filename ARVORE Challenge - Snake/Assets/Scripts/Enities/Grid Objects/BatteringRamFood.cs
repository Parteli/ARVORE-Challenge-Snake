using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamFood : Pickable
{
    public override string partURL
    {
        get{ return BatteringRamPart.PREFAB_URL; }        
    }
}
