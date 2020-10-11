using System;
using UnityEngine;
using UnityEngineX;
using TMPro;
using UnityEngine.Serialization;

public class TooltipItemDescription : GamePresentationBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private GameObject _background;

    protected override void OnGamePresentationUpdate() { }

    public void UpdateDescription(string text, Color color, bool addBG = false)
    {
        _descriptionText.text = text;
        _descriptionText.color = color;
        _background.SetActive(addBG);
    }
}