using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class DebugScreenMessage : MonoBehaviour
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

    private Vector2 destinedSizeDelta;

    private const float BASE_DURATION = 0.5f;
    private const float EXTRA_DURATION_PER_CHARACTER = 0.05f;
    private const string SCENENAME = "DebugScreenMessage";



    static Queue<string> queuedMessages = new Queue<string>();
    static bool IsDisplayingMessages { get; set; } = false;

    public static void DisplayMessage(string message)
    {
        queuedMessages.Enqueue(message);

        if (IsDisplayingMessages == false)
        {
            ProcessPendingMessages();
        }
    }
    public static void DisplayMessageFromThread(string message)
    {
        MainThreadService.AddMainThreadCallbackFromThread(() => DisplayMessage(message));
    }

    static private void ProcessPendingMessages()
    {
        IsDisplayingMessages = true;

        // load scene
        SceneService.LoadAsync(SCENENAME, LoadSceneMode.Additive, (scene) =>
        {
            // show message
            scene.FindRootObject<DebugScreenMessage>().DisplayText(queuedMessages.Dequeue(), ()=>
            {
                // repeat ?
                if(queuedMessages.Count > 0)
                {
                    ProcessPendingMessages();
                }
                else
                {
                    IsDisplayingMessages = false;
                }
            });

        }, allowMultiple: true);
    }

    private void Awake()
    {
        RectTransform imageRT = bgImage.rectTransform;
        destinedSizeDelta = imageRT.sizeDelta;

        imageRT.sizeDelta -= new Vector2(((1 - startHorizontal) * imageRT.rect.size.x), imageRT.sizeDelta.y);

        text.color = text.color.ChangedAlpha(0);
    }

    private void DisplayText(string message, Action onComplete)
    {
        text.text = message;

        Sequence sq = DOTween.Sequence();

        //Bg appear
        sq.Append(bgImage.rectTransform.DOSizeDelta(destinedSizeDelta, openDuration).SetEase(openEase));

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
            SceneService.UnloadAsync(gameObject.scene);
            onComplete();
        });
    }
}
