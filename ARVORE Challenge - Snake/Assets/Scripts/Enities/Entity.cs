using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    private Vector2 _coordinates;
    public Vector2 coordinates
    { get { return _coordinates; }
      set { _coordinates = value; } }

    [SerializeField]
    private Vector2 _orientation;
    public Vector2 orientation
    {
        get { return _orientation; }
        set { _orientation = value; }
    }

    public abstract void Contact();
}
