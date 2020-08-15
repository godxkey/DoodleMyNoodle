using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class HighlightElement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteLight _lightParams;

    private Tween _tween;

    [NonSerialized]
    public SpriteRenderer Target;
    public Action<HighlightElement> OnDestroyAction;
    public Action<HighlightElement> OnCompleteAction;

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void Play(float intensity, float loopDuration, bool startMidWay)
    {
        PlayInternal(intensity, loopDuration, -1, startMidWay);
    }

    public void Play(float intensity, float loopDuration, int loops, bool startMidWay)
    {
        PlayInternal(intensity, loopDuration, loops * 2, startMidWay);
    }

    private void PlayInternal(float intensity, float loopDuration, int demiLoops, bool startMidWay)
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }

        float start = startMidWay ? intensity : 0;
        float end = startMidWay ? 0 : intensity;

        _lightParams.Intensity = start;

        _tween = DOTween.To(() => _lightParams.Intensity, (x) => _lightParams.Intensity = x, end, loopDuration / 2f)
            .SetLoops(demiLoops, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => OnCompleteAction?.Invoke(this));
    }

    public void Stop()
    {
        _tween.Kill();
    }

    private void OnDestroy()
    {
        _tween.Kill();
        OnDestroyAction?.Invoke(this);
    }
}