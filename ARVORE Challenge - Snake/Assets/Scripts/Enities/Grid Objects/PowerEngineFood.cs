using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEngineFood : Pickable
{
    public override string partURL
    {
        get { return PowerEnginePart.PREFAB_URL; }
    }
}
