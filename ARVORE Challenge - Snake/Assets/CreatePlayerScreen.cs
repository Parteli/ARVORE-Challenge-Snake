using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerScreen : MonoBehaviour
{
    private GameController game;

    public RectTransform addNewText;

    private Player player;

    private float timer;

    private void Awake()
    {
        player = new Player();
        game = GameController.instance;   
    }

    public void Update()
    {
        if (timer != 0)
        {
            if (Time.timeSinceLevelLoad - timer > 3)
            {
                game.playerList.Add(player);
                Debug.Log("Configure Snake");
            }
        }
        if (game.playerList.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Start Game");
        }
    }

    public void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey)
        {

            string key = e.keyCode.ToString();

            Debug.Log(key);

            print("Hi");

        }

        if (!Input.anyKey)
        {
            player.leftKey = KeyCode.None;
            player.rightKey = KeyCode.None;
        }
    }
}
