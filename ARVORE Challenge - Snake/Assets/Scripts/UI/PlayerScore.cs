using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public RectTransform tagHolder;
    public Text scoreText;

    public int score = 0;

    public void Setup(Player player)
    {
        player.playerTag.transform.SetParent(tagHolder);
        scoreText.text = "0";
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }
}
