using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

public class DebugScreenMessage : MonoCoreService<DebugScreenMessage>
{
    [SerializeField, Header("Links"), FormerlySerializedAs("text")]
    private Text _text;
    [SerializeField, FormerlySerializedAs("bgImage")]
    private Image _bgImage;

    [SerializeField, Header("Animation Settings"), FormerlySerializedAs("openDuration")]
    private float _openDuration = 0.35f;
    [SerializeField, FormerlySerializedAs("openEase")]
    private Ease _openEase = Ease.OutQuad;
    [SerializeField, FormerlySerializedAs("startHorizontal")]
    private float _startHorizontal = 0.5f;

    [SerializeField, FormerlySerializedAs("hideDuration")]
    private float _hideDuration = 0.35f;
    [SerializeField, FormerlySerializedAs("hideEase")]
    private Ease _hideEase = Ease.InSine;

    private float _bgNormalAlpha;

    const float BASE_DURATION = 0.5f;
    const float EXTRA_DURATION_PER_CHARACTER = 0.05f;

    static Queue<string> s_queuedMessages = new Queue<string>();
    static bool s_isDisplayingMessages = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_queuedMessages.Clear();
    }

    public static void DisplayMessage(string message)
    {
        s_queuedMessages.Enqueue(message);

        if (Instance != null && s_isDisplayingMessages == false)
        {
            Instance.ProcessPendingMessages();
        }
    }
    public static void DisplayMessageFromThread(string message)
    {
        MainThreadService.AddMainThreadCallbackFromThread(() => DisplayMessage(message));
    }


    void ProcessPendingMessages()
    {
        s_isDisplayingMessages = true;

        // show message
        DisplayText(s_queuedMessages.Dequeue(), () =>
        {
            // repeat ?
            if (s_queuedMessages.Count > 0)
            {
                ProcessPendingMessages();
            }
            else
            {
                s_isDisplayingMessages = false;
            }
        });
    }

    private void Awake()
    {
        _bgNormalAlpha = _bgImage.color.a;

        gameObject.SetActive(false);

        if (s_isDisplayingMessages == false && s_queuedMessages.Count > 0)
        {
            ProcessPendingMessages();
        }
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        s_isDisplayingMessages = false;
    }

    void DisplayText(string message, Action onComplete)
    {
        gameObject.SetActive(true);

        _text.text = message;
        _text.color = _text.color.ChangedAlpha(0);
        _bgImage.rectTransform.localScale = new Vector3(_startHorizontal, 0, 1);

        Sequence sq = DOTween.Sequence();

        //Bg appear
        sq.Append(_bgImage.rectTransform.DOScale(1, _openDuration).SetEase(_openEase));
        sq.Join(_bgImage.DOFade(_bgNormalAlpha, _openDuration));

        //Text fade in
        sq.Insert(_openDuration * 0.6f, _text.DOFade(1, _openDuration * 0.4f));


        //Pause
        sq.AppendInterval(BASE_DURATION + message.Length * EXTRA_DURATION_PER_CHARACTER);


        //Bg disappear;
        sq.Append(_bgImage.DOFade(0, _hideDuration).SetEase(_hideEase));

        //Text disappear;
        sq.Join(_text.DOFade(0, _hideDuration).SetEase(_hideEase));


        sq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete();
        });
    }
}
