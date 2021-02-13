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
    /*
    public virtual string SaveInfo(string info ="")
    {
        info += "#c," + _coordinates.x + "," + _coordinates.y;
        info += "#o," + _orientation.x + "," + _orientation.y;
        info += "#" + GetType().ToString();
        //this one is the last so I can look for "#t," and directly verify the type

        System.Type type = System.Type.GetType("Entity");

        object mtType = System.Activator.CreateInstance(type);
       Debug.Log( ((SnakeBodyPart)mtType).prefab_url );

        return info;
    }

    public virtual string[][] LoadInfo(string info)
    {
        string[] split1 = info.Split("#"[0]);

        string[][] r = new string[split1.Length][];
 
        for (int i = 0; i < split1.Length; i++)
        {
            string[] split2 = split1[i].Split(","[0]);
            r[i] = new string[split2.Length];
            for (int j = 0; j < split2.Length; j++)
            {
                r[i][j] = split2[j];
                switch (split2[0])
                {
                    case "c":
                        _coordinates.x = int.Parse(split2[1]);
                        _coordinates.y = int.Parse(split2[2]);
                        break;
                    case "o":
                        _orientation.x = int.Parse(split2[1]);
                        _orientation.y = int.Parse(split2[2]);
                        break;
                }
            }
        }
        return r;
    }
    */
    public abstract void Contact();

}
