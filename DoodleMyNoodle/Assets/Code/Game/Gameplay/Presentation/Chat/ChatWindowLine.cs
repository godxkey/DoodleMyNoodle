using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatWindowLine : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color playerNameColor;
    [SerializeField] Color textColor;

    public void Fill(ChatLine chatLine)
    {
        text.text = string.Format("<color=#{2}>{0}:</color> <color=#{3}>{1}</color>",
            chatLine.ChatterName,
            chatLine.Message,
            ColorUtility.ToHtmlStringRGBA(playerNameColor),
            ColorUtility.ToHtmlStringRGBA(textColor));
    }
}
