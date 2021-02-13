using System.Collections.Generic;
using UnityEngine;

public class GridSlot
{
    private int _x;
    public virtual int x { get { return _x; } }

    private int _y;
    public virtual int y { get { return _y; } }

    private List<Entity> _users = null;
    public virtual List<Entity> users
    {
        get {
            if(_users == null) _users = new List<Entity>();
            return _users;
        }
        set { _users = value; }
    }

    private bool _hasChanged = false;
    public virtual bool hasChanged
    {
        get { return _hasChanged; }
        set { _hasChanged = value; }
    }

    private bool _isBorder = false;
    public virtual bool isBorder { get { return _isBorder; } }

    public GridSlot(int x, int y, bool isBorder)
    {
        this._x = x;
        this._y = y;
        this._isBorder = isBorder;
    }

    public virtual bool isInUse { get { return users.Count > 0; } }
    public virtual bool hasComflict { get { return users.Count > 1; } }

    public virtual void AddUser(Entity entity)
    {
        if (!users.Contains(entity))
        {
            users.Add(entity);
            hasChanged = true;
        }
    }
    public virtual void RemoveUser(Entity entity)
    {
        if (users.Remove(entity))
        {
            hasChanged = users.Count>0;
        }
    }
    public virtual void ClearUsers()
    {
        users.Clear();
        hasChanged = false;
    }

    public virtual void SolveConflicts(List<SlotActionTemplate> actions)
    {
        //nobody here
        if (!isInUse) return;

        //only 1 entity in the middle of the map
        if (!hasComflict && !isBorder) return;

        foreach (SlotActionTemplate action in actions)
            action.Evaluate(this);
    }
}