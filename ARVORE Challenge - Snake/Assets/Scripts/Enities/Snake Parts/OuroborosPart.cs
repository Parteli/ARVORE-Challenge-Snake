using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuroborosPart : SnakeBodyPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_Ouroboros";

    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/star";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Go full circle";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "You can eat the tail of other snake to steal her points." +
        " If you eat your onw tail you will get extra points instead.";

}

