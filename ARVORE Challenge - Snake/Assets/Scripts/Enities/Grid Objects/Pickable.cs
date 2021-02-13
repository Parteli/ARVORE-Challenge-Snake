using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : Entity
{
    public virtual string partURL
    {
        get {
            return "";
        }
    }

    public override void Contact()
    {
        GridSnake.instance.RemoveFromGrid(this);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
