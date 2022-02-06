using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class HighlightService : MonoCoreService<HighlightService>
{
    public enum Duration
    {
        Short,
        Long,
        UntilManuallyStopped
    }

    public enum FlickerSpeed
    {
        Slow,
        Fast
    }

    public enum Intensity
    {
        Normal,
        High
    }

    public enum AnimStart
    {
        NoHighlight,
        MidHighlight
    }

    [Serializable]
    public struct Params
    {
        public Duration Duration;
        public FlickerSpeed FlickerSpeed;
        public Intensity Intensity;
        public Color Color;
        public AnimStart AnimStart;

        public Params(Duration duration, FlickerSpeed flickerSpeed, Intensity intensity, Color color, AnimStart animStart)
        {
            Duration = duration;
            FlickerSpeed = flickerSpeed;
            Intensity = intensity;
            Color = color;
            AnimStart = animStart;
        }

        public static Params Default => new Params(Duration.Short, FlickerSpeed.Fast, Intensity.High, Color.white, AnimStart.NoHighlight);
    }

    [SerializeField] private HighlightElement _highlightPrefab;
    [SerializeField] private float _fastLoopDuration = 0.75f;
    [SerializeField] private float _slowLoopDuration = 1.5f;
    [SerializeField] private int _longLoops = 6;
    [SerializeField] private int _shortLoops = 3;
    [SerializeField] private float _normalIntensity = 1f;
    [SerializeField] private float _highIntensity = 2f;

    private List<HighlightElement> _highlightPool = new List<HighlightElement>();
    private List<HighlightElement> _activeHighlights = new List<HighlightElement>();
    private List<HighlightElement> _cachedList = new List<HighlightElement>();

    public override void Initialize(System.Action<ICoreService> onComplete) => onComplete(this);

    protected override void OnDestroy()
    {
        foreach (var item in _highlightPool)
        {
            item.OnDestroyAction = null;
            Destroy(item);
        }

        foreach (var item in _activeHighlights)
        {
            item.OnDestroyAction = null;
            Destroy(item.gameObject);
        }

        base.OnDestroy();
    }

    public static void HighlightSprite(SpriteRenderer spriteRenderer)
        => Instance.HighlightSpriteInternal(spriteRenderer, Params.Default);

    public static void HighlightSprite(SpriteRenderer spriteRenderer, Params param)
        => Instance.HighlightSpriteInternal(spriteRenderer, param);

    public static void StopHighlight(SpriteRenderer spriteRenderer)
        => Instance.StopHighlightInternal(spriteRenderer);

    private void HighlightSpriteInternal(SpriteRenderer spriteRenderer, Params param)
    {
        if (spriteRenderer == null)
        {
            Log.Error($"{nameof(spriteRenderer)} is null.");
            return;
        }

        HighlightElement newHighlight = TakeFromPool();
        Transform tr = newHighlight.transform;
        tr.SetParent(spriteRenderer.transform);
        tr.localPosition = Vector3.zero;
        tr.localRotation = Quaternion.identity;
        tr.localScale = Vector3.one;

        newHighlight.SetSprite(spriteRenderer.sprite, spriteRenderer.flipX, spriteRenderer.flipY);
        newHighlight.SetColor(param.Color);
        newHighlight.OnDestroyAction = OnHighlightDestroyed;
        newHighlight.OnCompleteAction = PutInPool;
        newHighlight.Target = spriteRenderer;

        float intensityValue;
        switch (param.Intensity)
        {
            default:
            case Intensity.Normal:
                intensityValue = _normalIntensity;
                break;
            case Intensity.High:
                intensityValue = _highIntensity;
                break;
        }

        float loopDuration;
        switch (param.FlickerSpeed)
        {
            default:
            case FlickerSpeed.Slow:
                loopDuration = _slowLoopDuration;
                break;
            case FlickerSpeed.Fast:
                loopDuration = _fastLoopDuration;
                break;
        }
        
        bool startMidway = param.AnimStart == AnimStart.MidHighlight;

        switch (param.Duration)
        {
            case Duration.Short:
                newHighlight.Play(intensityValue, loopDuration, _shortLoops, startMidway);
                break;
            case Duration.Long:
                newHighlight.Play(intensityValue, loopDuration, _longLoops, startMidway);
                break;
            case Duration.UntilManuallyStopped:
                newHighlight.Play(intensityValue, loopDuration, startMidway);
                break;
        }
    }

    private void StopHighlightInternal(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
            return;

        // find all highlights to stop
        for (int i = 0; i < _activeHighlights.Count; i++)
        {
            if (_activeHighlights[i].Target == spriteRenderer)
            {
                _cachedList.Add(_activeHighlights[i]);
            }
        }

        // stop all
        foreach (var item in _cachedList)
        {
            item.Stop();
            PutInPool(item);
        }

        _cachedList.Clear();
    }

    private void OnHighlightDestroyed(HighlightElement element)
    {
        _activeHighlights.Remove(element);
        _highlightPool.Remove(element);
    }

    private HighlightElement TakeFromPool()
    {
        HighlightElement element;

        if (_highlightPool.Count > 0)
        {
            element = _highlightPool.Pop();
            element.gameObject.SetActive(true);
        }
        else
        {
            element = Instantiate(_highlightPrefab);
        }

        _activeHighlights.Add(element);

        return element;
    }

    private void PutInPool(HighlightElement element)
    {
        _activeHighlights.Remove(element);

        element.transform.SetParent(null);
        element.gameObject.SetActive(false);
        _highlightPool.Add(element);
    }
}