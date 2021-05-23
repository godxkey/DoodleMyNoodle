using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class InfoTextDisplay : GamePresentationSystem<InfoTextDisplay>
{
    [SerializeField] private TextMeshProUGUI _textObject; 

    protected override void OnGamePresentationUpdate()
    {
        
    }

    // TODO : add possibility to format (color, bold, etc.)
    public void SetText(TextData infoText, float duration = 1)
    {
        _textObject.text = infoText.ToString();
        _textObject.gameObject.SetActive(true);

        // TODO support duration
    }

    public void ForceHideText()
    {
        _textObject.gameObject.SetActive(false);
    }
}