using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSnakeUI : MonoBehaviour
{
    [Header("Add Snake Parts to an\nobject in order to make " +
        "\nthem selectable in game")]
    public Transform snakePartsReferences;

    [HideInInspector]
    public Player player;

    [SerializeField] private Slider downButton;
    [SerializeField] private Image colorDisplay;
    [SerializeField] private Image part1Icon;
    [SerializeField] private Text part1Text;
    [SerializeField] private Image part2Icon;
    [SerializeField] private Text part2Text;
    [SerializeField] private Image part3Icon;
    [SerializeField] private Text part3Text;
    [SerializeField] private Text confirmText;
    [SerializeField] private RectTransform descriptionObj;
    [SerializeField] private Text descriptionText;
    private CreatePlayerScreen createPlayerScreen;

    private int partHead;
    private int partBody;
    private int partTail;

    private SnakeBodyPart[] typeList;

    private void Awake()
    {
        createPlayerScreen = GetComponentInParent<CreatePlayerScreen>();
        typeList = snakePartsReferences.GetComponents<SnakeBodyPart>();

    }

    public void Setup(Player player)
    {
        this.player = player;
        gameObject.SetActive(true);

        string n = Utility.LastLetterOf(player.leftKey.ToString());
        downButton.GetComponentInChildren<Text>().text = n;

        n = Utility.LastLetterOf(player.rightKey.ToString());
        confirmText.text = "Press " + n + " to Confirm";

        partHead = 0;
        ChangeHead();
        partBody = 1;
        ChangeBody();
        partTail = 2;
        ChangeTail();
        downButton.value = 1;
        PressColor();
    }

    private void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(player.leftKey)) NextLine();

        if (Input.GetKeyDown(player.rightKey))
        {
            switch (downButton.value)
            {
                case 1: PressColor();
                    break;
                case 2: ChangeHead();
                    break;
                case 3: ChangeBody();
                    break;
                case 4: ChangeTail();
                    break;
                case 5:
                    ConfirmCreation();
                    break;
            }
        }
    }

    private void ChangeHead()
    {
        if (partHead >= typeList.Length - 1) partHead = 0;
        else partHead++;

        part1Icon.sprite = Resources.Load<Sprite>(typeList[partHead].icon_url);
        part1Text.text = typeList[partHead].small_text;
    }
    private void ChangeBody()
    {
        if (partBody >= typeList.Length -1) partBody = 0;
        else partBody++;

        part2Icon.sprite = Resources.Load<Sprite>(typeList[partBody].icon_url);
        part2Text.text = typeList[partBody].small_text;
    }
    private void ChangeTail()
    {
        if (partTail >= typeList.Length-1) partTail = 0;
        else partTail++;

        part3Icon.sprite = Resources.Load<Sprite>(typeList[partTail].icon_url);
        part3Text.text = typeList[partTail].small_text;
    }

    private void NextLine()
    {
        if (downButton.value >= 5) downButton.value = 1;
        else downButton.value++;
    }

    private void PressColor()
    {
        player.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
        colorDisplay.color = player.color;
    }


    private void ConfirmCreation()
    {
        // Create tags ----------------------------------
        Transform obj = Instantiate(Resources.Load<Transform>("Prefabs/Player Tag"));
        player.playerTag = obj.GetComponent<PlayerTag>();
        player.playerTag.Setup(player.color,
                            Utility.LastLetterOf(player.leftKey.ToString()),
                            Utility.LastLetterOf(player.rightKey.ToString()),
                            ReverseColor(player.color));

        // Snake data --------------------------------------------------
        List<string> data = new List<string>();
        data.Add(typeList[partHead].prefab_url);
        data.Add(typeList[partBody].prefab_url);
        data.Add(typeList[partTail].prefab_url);
        player.snakeData = data;

        //finish ---------------
        gameObject.SetActive(false);//deactive obj first 
        createPlayerScreen.FinishPlayer(player);

        player = null;
    }


    private Color ReverseColor(Color color)
    {
        float hue;
        float sat;
        float val;

        Color.RGBToHSV(color, out hue, out sat, out val);

        return Color.HSVToRGB( (hue+0.5f)%1, sat, val);
    }
}

/*
 *  Icons by
 <div>Icons made by <a href="https://www.flaticon.com/authors/gregor-cresnar" title="Gregor Cresnar">Gregor Cresnar</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
 */
