

using UnityEngine;

public static class Utility
{

    public delegate void GetBoolDelegate(bool value);
    public delegate void GetSnakeDelegate(Snake value);
    public delegate void GetPickableDelegate(Pickable value);


    public static string LastLetterOf(string text)
    {
        return text[text.Length - 1].ToString();
    }


    public static void SetColorForAll(Transform transform, Color color)
    {
        foreach (Renderer m in transform.GetComponentsInChildren<Renderer>(true))
        {
            m.material.color = color;
        }
    }

    public static Vector2 BestCardinalDirection(Vector2 distance)
    {
        Vector2 vec = Vector2.zero;
        if (Mathf.Abs(distance.x) >= Mathf.Abs(distance.y))
            vec.x = Mathf.Sign(distance.x);
        else vec.y = Mathf.Sign(distance.y);
        return vec;
    }
}




public enum CardinalDirections { North, South, West, East }