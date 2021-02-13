using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamPart : HeadPowerPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_BatteringRam";
    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/download-1";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Run over snakes";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "It allows to pass over 1 snake body.";
}
