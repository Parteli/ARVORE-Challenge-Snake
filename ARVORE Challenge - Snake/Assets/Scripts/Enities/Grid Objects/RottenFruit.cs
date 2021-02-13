using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RottenFruit : Pickable
{
    public override string partURL
    {
        get { return WrongBodyPart.PREFAB_URL; }
    }

}
