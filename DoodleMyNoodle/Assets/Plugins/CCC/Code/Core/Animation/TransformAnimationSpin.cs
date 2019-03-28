using UnityEngine;
using DG.Tweening;

public class TransformAnimationSpin : MonoBehaviour
{
    [Header("Settings")]
    public bool timeScaleIndependant = true;

    [Header("Behaviour")]
    public EnableBehaviour onEnable = EnableBehaviour.Restart;
    public DisableBehaviour onDisable = DisableBehaviour.Stop;


    [Header("Animation")]
    public float cyclesPerSecond = 1;
    public bool clockwise = true;
    public Ease ease = Ease.Linear;

    public enum EnableBehaviour { Nothing, Resume, Restart }
    public enum DisableBehaviour { Nothing, Pause, Stop }

    private Tween _tween;
    private Transform _tr;
    private Quaternion _initialLocalRot;

    void Awake()
    {
        _tr = transform;
        _initialLocalRot = _tr.localRotation;
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

    public bool IsPlaying => IsTweenActive && _tween.IsPlaying();
    private bool IsTweenActive => _tween != null && _tween.IsActive();

    public void StartAnimation()
    {
        KillTween();

        if (cyclesPerSecond == 0)
        {
            return;
        }

        float duration = 1f / cyclesPerSecond;

        _tween = _tr.DORotate(Vector3.forward * (clockwise ? -360f : 360f), duration, RotateMode.LocalAxisAdd)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(timeScaleIndependant);
    }

    public void Stop()
    {
        KillTween();
        _tr.localRotation = _initialLocalRot;
    }

    public void Restart()
    {
        if (IsTweenActive)
            _tween.Restart();
        else
            StartAnimation();
    }

    public void Pause()
    {
        if (IsPlaying)
            _tween.Pause();
    }

    public void Play()
    {
        if (IsTweenActive)
            _tween.Play();
        else
            StartAnimation();
    }

    void KillTween()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();
        _tween = null;
    }

}

