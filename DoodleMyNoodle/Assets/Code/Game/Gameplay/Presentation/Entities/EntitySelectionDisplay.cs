using System;
using UnityEngine;
using UnityEngineX;

public class EntitySelectionDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _overingHighlightColor = Color.white;

    public void StartOveringOnDisplay()
    {
        if (_spriteRenderer)
        {
            var highlightParams = HighlightService.Params.Default;
            highlightParams.Color = _overingHighlightColor;
            highlightParams.FlickerSpeed = HighlightService.FlickerSpeed.Fast;
            highlightParams.Intensity = HighlightService.Intensity.High;

            HighlightService.HighlightSprite(_spriteRenderer, highlightParams);
        }
    }

    public void StopOveringOnDisplay()
    {
        if (_spriteRenderer)
        {
            HighlightService.StopHighlight(_spriteRenderer);
        }
    }
}