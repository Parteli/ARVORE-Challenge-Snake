using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongBodyPart : SnakeBodyPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_WrongPart";

    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/star";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Bad apple";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "You ate something that wasn't yours and got slower than usual.";

    private static float SPEED_DEBUFF = -0.4f;

    private void Start()
    {
        snake.AddSpeedBuff(SPEED_DEBUFF);
    }

}
