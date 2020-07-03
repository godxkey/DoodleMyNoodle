using System;
using UnityEngine;
using UnityEngineX;
using TMPro;

public class TooltipItemDescription : GamePresentationBehaviour
{
    [SerializeField]
    private TextMeshProUGUI DescriptionText;

    protected override void OnGamePresentationUpdate() { }

    public void UpdateDescription(string text, Color color)
    {
        DescriptionText.text = text;
        DescriptionText.color = color;
    }
}