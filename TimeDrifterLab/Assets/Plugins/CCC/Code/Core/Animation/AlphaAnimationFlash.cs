using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class AlphaAnimationFlash : MonoBehaviour
{
    [Header("Settings")]
    public bool timeScaleIndependant = false;
    public ComponentType componentType = ComponentType.Nothing;

    [Header("Behaviour")]
    public EnableBehaviour onEnable = EnableBehaviour.Restart;
    public DisableBehaviour onDisable = DisableBehaviour.Stop;

    [Header("Animation")]
    public bool transparentAtStart = false;
    public float fadeDuration = 1f;
    public Ease ease = Ease.Linear;

    public enum ComponentType { Nothing = 0, Text = 1, Image = 2, Sprite = 3, CanvasGroup = 4 }
    public enum EnableBehaviour { Nothing, Resume, Restart }
    public enum DisableBehaviour { Nothing, Pause, Stop }

    private Tween tween;
    public float OriginalAlpha { get; set; }

    void Awake()
    {
        OriginalAlpha = GetCurrentAlpha();
    }

    void OnEnable()
    {
        switch (onEnable)
        {
            case EnableBehaviour.Restart:
                Restart();
                break;
            case EnableBehaviour.Resume:
                Play();
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

    public void Stop()
    {
        if (tween != null && tween.IsActive())
            tween.Kill();
    }

    public void Play()
    {
        if (tween != null && tween.IsActive())
            tween.Play();
        else
            StartAnimation();
    }

    public void Pause()
    {
        if (tween != null && tween.IsPlaying())
            tween.Pause();
    }

    public void Restart()
    {
        if (tween != null && tween.IsPlaying())
            tween.Restart();
        else
            StartAnimation();
    }

    void StartAnimation()
    {
        if (componentType == ComponentType.Nothing)
            return;

        Stop();

        float begin = transparentAtStart ? 0 : OriginalAlpha;
        float end = transparentAtStart ? OriginalAlpha : 0;

        switch (componentType)
        {
            case ComponentType.Text:
                {
                    Text text = GetComponent<Text>();
                    if (text == null)
                        return;

                    text.color = text.color.ChangedAlpha(begin);
                    tween = text.DOFade(end, fadeDuration);

                    break;
                }
            case ComponentType.Image:
                {
                    Image image = GetComponent<Image>();
                    if (image == null)
                        return;

                    image.SetAlpha(begin);
                    tween = image.DOFade(end, fadeDuration);

                    break;
                }
            case ComponentType.Sprite:
                {
                    SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
                    if (sprRenderer == null)
                        return;

                    sprRenderer.SetAlpha(begin);
                    tween = sprRenderer.DOFade(end, fadeDuration);

                    break;
                }
            case ComponentType.CanvasGroup:
                {
                    CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                        return;

                    canvasGroup.alpha = begin;
                    tween = canvasGroup.DOFade(end, fadeDuration);

                    break;
                }
            default:
                break;
        }

        tween?.SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }

    float GetCurrentAlpha()
    {
        switch (componentType)
        {
            case ComponentType.Text:
                {
                    Text text = GetComponent<Text>();
                    if (text != null)
                        return text.color.a;
                    break;
                }
            case ComponentType.Image:
                {
                    Image image = GetComponent<Image>();
                    if (image != null)
                        return image.color.a;
                    break;
                }
            case ComponentType.Sprite:
                {
                    SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
                    if (sprRenderer != null)
                        return sprRenderer.color.a;
                    break;
                }
            case ComponentType.CanvasGroup:
                {
                    CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                        return canvasGroup.alpha;
                    break;
                }
        }
        Debug.LogError("Wrong 'ComponentType' on " + gameObject.name + "'s " + nameof(AlphaAnimationFlash));
        return 1;
    }
}
