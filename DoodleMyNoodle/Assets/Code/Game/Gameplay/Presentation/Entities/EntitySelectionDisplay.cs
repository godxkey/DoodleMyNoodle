using System;
using UnityEngine;
using UnityEngineX;

public class EntitySelectionDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private DoodleDisplay _doodleDisplay;
    [SerializeField] private Color _overingHighlightColor = Color.white;

    public void StartOveringOnDisplay()
    {
        if (_doodleDisplay)
        {
            var highlightParams = HighlightService.Params.Default;
            highlightParams.Color = _overingHighlightColor;
            highlightParams.FlickerSpeed = HighlightService.FlickerSpeed.Fast;
            highlightParams.Intensity = HighlightService.Intensity.High;

            HighlightService.HighlightSprite(_doodleDisplay.SpriteRenderer, highlightParams);
        }
    }

    public void StopOveringOnDisplay()
    {
        if (_doodleDisplay)
        {
            HighlightService.StopHighlight(_doodleDisplay.SpriteRenderer);
        }
    }

    protected override void OnGamePresentationUpdate() { }
}