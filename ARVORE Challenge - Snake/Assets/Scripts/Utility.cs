

using UnityEngine;

public static class Utility
{

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
}