using UnityEngine;
using DG.Tweening;

public class TransformAnimationFloating : MonoBehaviour
{
    [Header("Settings")]
    public bool timeScaleIndependant = true;

    [Header("Behaviour")]
    public EnableBehaviour onEnable = EnableBehaviour.Restart;
    public DisableBehaviour onDisable = DisableBehaviour.Stop;


    [Header("Animation")]
    public float cycleDuration = 1;
    public float maxSize = 1.15f;
    public float minSize = 0.85f;
    public Ease ease = Ease.InOutSine;

    public enum EnableBehaviour { Nothing, Resume, Restart }
    public enum DisableBehaviour { Nothing, Pause, Stop }

    private Tween tween;
    private Transform tr;
    private float normalScale = 1;

    void Awake()
    {
        tr = transform;
        normalScale = tr.localScale.x;
    }

    void OnEnable()
    {
        switch (onEnable)
        {
            case EnableBehaviour.Resume:
                Play();
                break;
            case EnableBehaviour.Restart:
                Restart();
                break;
        }
    }

    void OnDisable()
    {
        switch (onDisable)
        {
            case DisableBehaviour.Pause:
                Pause();
                break;
            case DisableBehaviour.Stop:
                Stop();
                break;
        }
    }

    public bool IsPlaying => IsTweenActive && tween.IsPlaying();
    private bool IsTweenActive => tween != null && tween.IsActive();

    public void StartAnimation()
    {
        KillTween();

        tr.localScale = Vector3.one * minSize;

        float duration = Mathf.Max(0.02f, cycleDuration / 2);
        tween = tr.DOScale(maxSize, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(timeScaleIndependant);
        tween.Goto(duration / 2, true);
    }

    public void Stop()
    {
        KillTween();
        tr.localScale = Vector3.one * normalScale;
    }

    public void Restart()
    {
        if (IsTweenActive)
            tween.Restart();
        else
            StartAnimation();
    }

    public void Pause()
    {
        if (IsPlaying)
            tween.Pause();
    }

    public void Play()
    {
        if (IsTweenActive)
            tween.Play();
        else
            StartAnimation();
    }

    void KillTween()
    {
        if (tween != null && tween.IsActive())
            tween.Kill();
        tween = null;
    }

}

