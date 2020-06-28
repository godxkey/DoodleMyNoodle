using System;
using UnityEngine;
using UnityEngineX;
using TMPro;

public class TooltipItemDescription : GamePresentationBehaviour
{
    public TextMeshProUGUI DescriptionText;

    protected override void OnGamePresentationUpdate() { }

    public void UpdateDescription(string text, Color color)
    {
        DescriptionText.text = text;
        DescriptionText.color = color;
    }
}