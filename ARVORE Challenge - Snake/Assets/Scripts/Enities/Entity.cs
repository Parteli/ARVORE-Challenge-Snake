using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected Vector2 _coordinates;
    public virtual Vector2 coordinates
    { get { return _coordinates; }
      set { _coordinates = value; } }

    [SerializeField]
    protected Vector2 _orientation;
    public virtual Vector2 orientation
    {
        get { return _orientation; }
        set { _orientation = value; }
    }

    public abstract void Contact();
}
