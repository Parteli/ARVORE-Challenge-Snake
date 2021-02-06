using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTag : MonoBehaviour
{
    private Color color { get { return icon.color; } }
    private Image icon;
    [SerializeField]
    private Text leftKeyText;
    [SerializeField]
    private Text rightKeyText;

    public void Setup(Color color, string leftKey, string rightKey, Color negative)
    {
        icon = GetComponent<Image>();
        icon.color = color;

        leftKeyText.text = leftKey;
        leftKeyText.color = negative;

        rightKeyText.text = rightKey;
        rightKeyText.color = negative;
    }
}
