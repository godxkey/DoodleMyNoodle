using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngineX;

public class ChatWindow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maximumChatLines;
    [SerializeField] AudioPlayable newLineSfx;

    [Header("References")]
    [SerializeField] ChatWindowLine chatLinePrefab;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatLineContainer;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject panel;


    public bool displayed { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    public bool focused
    {
        get { return _focused; }
        set
        {
            _focused = value;
            inputField.gameObject.SetActive(value);

            if (_focused)
            {
                scrollRect.verticalScrollbar = scrollBar;
                inputField.ActivateInputField();
                inputField.Select();
            }
            else
            {
                if (EventSystem.current == inputField.gameObject)
                    EventSystem.current.SetSelectedGameObject(null);
                scrollRect.verticalScrollbar = null;
                scrollBar.gameObject.SetActive(false);
            }
        }
    }
    public string inputText { get { return inputField.text; } set { inputField.text = value; } }
    bool canScroll => focused;

    bool _focused = false;
    List<ChatWindowLine> chatLineGameObjects = new List<ChatWindowLine>();

    public void AddLine(ChatLine chatLine)
    {
        if (chatLineGameObjects.Count >= maximumChatLines)
        {
            // we're maxed out ! move the first line down
            chatLineGameObjects[0].transform.SetAsLastSibling();
            chatLineGameObjects.MoveLast(0);
        }
        else
        {
            chatLineGameObjects.Add(Instantiate(chatLinePrefab, chatLineContainer));
        }

        chatLineGameObjects.Last().Fill(chatLine);

        DefaultAudioSourceService.Instance.PlayStaticSFX(newLineSfx);
    }

    void Update()
    {
        if (!_focused)
            ResetScroll();
    }

    public void ResetScroll()
    {
        scrollRect.normalizedPosition = Vector2.zero; // meaning (0, 0)
    }
}
