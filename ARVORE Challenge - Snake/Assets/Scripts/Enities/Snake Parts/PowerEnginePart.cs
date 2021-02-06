using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEnginePart : SnakeBodyPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_PowerEngine";

    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/star";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Give me more speed";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "Makes the snake go faster and more than compensate for the increase in weight.";

    private static float SPEED_BUFF = 1f;

    private void Start()
    {
        snake.AddSpeedBuff(SPEED_BUFF);
    }
    private void OnDestroy()
    {
        snake.RemoveSpeedBuff(SPEED_BUFF);
    }


}
