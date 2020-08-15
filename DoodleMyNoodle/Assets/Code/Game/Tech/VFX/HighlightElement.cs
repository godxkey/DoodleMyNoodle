using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class HighlightElement : MonoBehaviour
{
    [SerializeField] private Light2D _light;

    private Tween _tween;

    [NonSerialized]
    public SpriteRenderer Target;
    public Action<HighlightElement> OnDestroyAction;
    public Action<HighlightElement> OnCompleteAction;

    public void SetSprite(Sprite sprite)
    {
        SetLightSprite(_light, sprite);
    }

    public void Play(float intensity, float loopDuration)
    {
        PlayInternal(intensity, loopDuration, -1);
    }

    public void Play(float intensity, float loopDuration, int loops)
    {
        PlayInternal(intensity, loopDuration, loops * 2);
    }

    private void PlayInternal(float intensity, float loopDuration, int demiLoops)
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }

        _tween = DOTween.To(() => _light.intensity, (x) => _light.intensity = x, intensity, loopDuration / 2f)
            .SetLoops(demiLoops, LoopType.Yoyo)
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


    private static FieldInfo s_lightCookieSpriteMember;
    private static void SetLightSprite(Light2D light, Sprite sprite)
    {
        if (s_lightCookieSpriteMember == null)
        {
            s_lightCookieSpriteMember = typeof(Light2D).GetField("m_LightCookieSprite", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        s_lightCookieSpriteMember.SetValue(light, sprite);
    }
}