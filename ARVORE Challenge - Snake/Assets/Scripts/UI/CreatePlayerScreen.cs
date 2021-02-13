using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerScreen : MonoBehaviour
{
    private GameController game;

    [SerializeField] private Text mainText;
    [TextArea]
    public string addNewTxt;
    [TextArea]
    public string isFullTxt;

    [SerializeField] private Text leftKeyHoldText;
    [SerializeField] private Text rightKeyHoldText;
    [SerializeField] private RectTransform keyHoldFill;


    [SerializeField] private RectTransform playerCreationArea;
    [SerializeField] private RectTransform playerReadyArea;

    [SerializeField] private Text matchStartText;

    private BuildSnakeUI[] buildSnakeList;

    private float timer;

    private KeyCode leftKey;
    private KeyCode rightKey;

    private void Awake()
    {
        game = GameController.instance;
        mainText.text = addNewTxt;

        buildSnakeList = GetComponentsInChildren<BuildSnakeUI>(true);
        foreach (BuildSnakeUI b in buildSnakeList) b.gameObject.SetActive(false);

        matchStartText.gameObject.SetActive(false);
        ClearKeys();
    }
    
    public void Update()
    {
        if (timer != 0 && leftKey != KeyCode.None && rightKey != KeyCode.None)
        {
            float t = (Time.timeSinceLevelLoad - timer);
            keyHoldFill.localScale = new Vector2(t, 1);

            if (t >= 1)
            {
                OpenBuilder();
                timer = 0;
            }
        }

        if (!Input.anyKey)
        {
            timer = 0;
            ClearKeys();
        }
        else 
        {
            if (!matchStartText.gameObject.activeSelf) return;

            if ( Input.GetKeyDown(KeyCode.Return) )
            {
                game.StartMatch();
                gameObject.SetActive(false);
                enabled = false;
            }
        }
    }

    private void OnGUI()
    {
        Event e = Event.current;
        
        //is key input?
        if (!e.isKey) return;

        //there is a place to build the snake?
        if (!BuilderSlotAvailable()) return;

        //is this key available?
        if (!CheckPlayerKeys(e.keyCode)) return;

        //is the key a letter or an alphaNumber
        if (e.keyCode.ToString().Length == 1 ||
            e.keyCode.ToString().Contains("Alpha"))
        {
            //left first
            if (leftKey == KeyCode.None) SetLeftKey(e.keyCode);
            else if (rightKey == KeyCode.None && e.keyCode != leftKey)
            {
                SetRightKey(e.keyCode);
                timer = Time.timeSinceLevelLoad;
            }
        }
    }
    
    public void FinishPlayer(Player player)
    {
        player.playerTag.transform.SetParent(playerReadyArea);
        mainText.text = addNewTxt;
        //OrganizePlayerTags();

        int count = 0;
        foreach (BuildSnakeUI b in buildSnakeList)
        {
            if (!b.gameObject.activeSelf) count++;
        }

        if(count == buildSnakeList.Length)
            matchStartText.gameObject.SetActive(true);
    }
    /*
    private void OrganizePlayerTags()
    {
        PlayerTag[] tagList = GetComponentsInChildren<PlayerTag>(true);

        GridLayoutGroup grid = playerReadyArea.GetComponent<GridLayoutGroup>();

        int length = Mathf.CeilToInt( tagList.Length * 0.5f);

        float usableWidth = playerReadyArea.rect.width - grid.padding.left - grid.padding.right;
       
        float width = (usableWidth -(length-1)*grid.spacing.x) / length;

        if (width < grid.cellSize.x) 
            grid.cellSize = new Vector2(width, grid.cellSize.y);
    }
    */
    private void OpenBuilder()
    {
        Player player = new Player();

        player.leftKey = leftKey;
        player.rightKey = rightKey;

        game.playerList.Add(player);

        for(int i = 0; i < buildSnakeList.Length; i++)
        {
            if (!buildSnakeList[i].gameObject.activeSelf)
            {
                buildSnakeList[i].Setup(player);
                break;
            }
        }

        if (!BuilderSlotAvailable()) mainText.text = isFullTxt;
        matchStartText.gameObject.SetActive(false);

        ClearKeys();
    }


    #region Utility
    private void SetLeftKey(KeyCode k = KeyCode.None)
    {
        if (k == KeyCode.None)
        { leftKeyHoldText.text = ""; }
        else
        {
            leftKeyHoldText.text = Utility.LastLetterOf(k.ToString());
        }

        leftKey = k;
    }
    private void SetRightKey(KeyCode k = KeyCode.None)
    {
        if (k == KeyCode.None)
        { rightKeyHoldText.text = ""; }
        else
        {
            rightKeyHoldText.text = Utility.LastLetterOf(k.ToString());
        }

        rightKey = k;
    }
    private void ClearKeys()
    {
        SetRightKey();
        SetLeftKey();
        keyHoldFill.localScale = new Vector2(0, 1);
    }
    private bool CheckPlayerKeys(KeyCode key)
    {
        foreach (Player p in game.playerList)
        {
            if (p.leftKey == key || p.rightKey == key)
            { return false; }
        }

        return true;
    }

    private bool BuilderSlotAvailable()
    {
        foreach (BuildSnakeUI b in buildSnakeList)
        {
            if (!b.gameObject.activeSelf) return true;
        }
        return false;
    }

    #endregion
}
