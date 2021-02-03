using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance = null;
    public static GameController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<GameController>();
                    _instance.name = "__GameController__";
                }
            }

            return _instance;
        }
    }

    private GridSnake gridSnake;

    private List<Snake> snakeList;

    [HideInInspector]
    public List<Player> playerList;

    private List<SlotActionTemplate> actionList;


    private void Awake()
    {
        gridSnake = FindObjectOfType<GridSnake>();
        actionList = new List<SlotActionTemplate>();
        actionList.AddRange(GetComponentsInChildren<SlotActionTemplate>(true));

        bool actionBorder = false;
        foreach (SlotActionTemplate act in actionList)
            if (act is BorderLineAction) actionBorder = true;

        if (!actionBorder) actionList.Add(new DeathBorderAction() );

        // players ??????------------------------
        snakeList = new List<Snake>();
        snakeList.AddRange( FindObjectsOfType<Snake>(true) ) ;
        //---------------------------------------------------
    }


    private void LateUpdate()
    {
        SolveGridConflicts();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            Application.Quit();
    }


    private void SolveGridConflicts()
    {
        List<GridSlot> cases = gridSnake.FindConflicts();

        foreach (GridSlot slot in cases)
        {
            CallActionsOnSlot(slot);
        }

    }
    public void CallActionsOnSlot(GridSlot slot)
    {
        foreach (SlotActionTemplate action in actionList)
            action.Evaluate(slot);
    }
}













