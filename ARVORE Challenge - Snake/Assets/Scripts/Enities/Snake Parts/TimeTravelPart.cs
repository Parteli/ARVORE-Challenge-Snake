using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelPart : SnakeBodyPart
{
    //I need to find a better way to do this...
    //I should use custom DATA
    public override string prefab_url { get { return PREFAB_URL; } }
    public static new string PREFAB_URL = "Prefabs/BodyPart_TimeTravel";

    public override string icon_url { get { return ICON_URL; } }
    public static new string ICON_URL = "Icons/reload";

    public override string small_text { get { return SMALL_TEXT; } }
    public static new string SMALL_TEXT = "Erase mistakes";

    public override string description { get { return DESCRIPTION; } }
    public static new string DESCRIPTION = "When you die the time will run backwards until the moment you got this power.";


    private void Start()
    {
        //saves information
    }

    public override void Contact()
    {
        //apply information
    }
}
