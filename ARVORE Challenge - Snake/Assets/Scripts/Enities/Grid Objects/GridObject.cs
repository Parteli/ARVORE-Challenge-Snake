using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject : Entity
{

    public abstract void PlacedOnGrid();

    public abstract void RemovedFromGrid();
}
