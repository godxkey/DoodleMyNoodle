using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class DebugScreenMessage : MonoCoreService<DebugScreenMessage>
{
    [SerializeField, Header("Links")]
    private Text text;
    [SerializeField]
    private Image bgImage;

    [SerializeField, Header("Animation Settings")]
    private float openDuration = 0.35f;
    [SerializeField]
    private Ease openEase = Ease.OutQuad;
    [SerializeField]
    private float startHorizontal = 0.5f;

    [SerializeField]
    private float hideDuration = 0.35f;
    [SerializeField]
    private Ease hideEase = Ease.InSine;

    private float bgNormalAlpha;

    const float BASE_DURATION = 0.5f;
    const float EXTRA_DURATION_PER_CHARACTER = 0.05f;

    static Queue<string> queuedMessages = new Queue<string>();
    static bool isDisplayingMessages = false;

    public static void DisplayMessage(string message)
    {
        queuedMessages.Enqueue(message);

        if (Instance != null && isDisplayingMessages == false)
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
        isDisplayingMessages = true;

        // show message
        DisplayText(queuedMessages.Dequeue(), () =>
        {
            // repeat ?
            if (queuedMessages.Count > 0)
            {
                ProcessPendingMessages();
            }
            else
            {
                isDisplayingMessages = false;
            }
        });
    }

    private void Awake()
    {
        RectTransform imageRT = bgImage.rectTransform;

        bgNormalAlpha = bgImage.color.a;

        gameObject.SetActive(false);

        if (isDisplayingMessages == false && queuedMessages.Count > 0)
        {
            ProcessPendingMessages();
        }
    }

    void DisplayText(string message, Action onComplete)
    {
        gameObject.SetActive(true);

        text.text = message;
        text.color = text.color.ChangedAlpha(0);
        bgImage.rectTransform.localScale = new Vector3(startHorizontal, 0, 1);

        Sequence sq = DOTween.Sequence();

        //Bg appear
        sq.Append(bgImage.rectTransform.DOScale(1, openDuration).SetEase(openEase));
        sq.Join(bgImage.DOFade(bgNormalAlpha, openDuration));

        //Text fade in
        sq.Insert(openDuration * 0.6f, text.DOFade(1, openDuration * 0.4f));


        //Pause
        sq.AppendInterval(BASE_DURATION + message.Length * EXTRA_DURATION_PER_CHARACTER);


        //Bg disappear;
        sq.Append(bgImage.DOFade(0, hideDuration).SetEase(hideEase));

        //Text disappear;
        sq.Join(text.DOFade(0, hideDuration).SetEase(hideEase));


        sq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete();
        });
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);
    }
}
