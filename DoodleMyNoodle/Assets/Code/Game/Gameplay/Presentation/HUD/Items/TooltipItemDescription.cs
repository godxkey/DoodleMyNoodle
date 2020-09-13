using System;
using UnityEngine;
using UnityEngineX;
using TMPro;

public class TooltipItemDescription : GamePresentationBehaviour
{
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private GameObject Background;

    protected override void OnGamePresentationUpdate() { }

    public void UpdateDescription(string text, Color color, bool addBG = false)
    {
        DescriptionText.text = text;
        DescriptionText.color = color;

        if (addBG)
        {
            Background.SetActive(true);
        }
    }
}