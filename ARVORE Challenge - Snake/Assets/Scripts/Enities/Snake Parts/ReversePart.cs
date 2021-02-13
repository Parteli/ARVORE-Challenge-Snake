using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversePart : HeadPowerPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_Reverse";

    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/star";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Heads or tails?";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "If you hit something, instead of dying your tail " +
        "will became the head and move on the oposite direction.";

    public override void Contact()
    {
        //change snake
    }

}
