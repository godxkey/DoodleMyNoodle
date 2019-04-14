using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatWindowController : GameMonoBehaviour
{
    public float inactivityCloseDelay = 5f;
    [SerializeField] ChatWindow _chatWindowPrefab;

    protected ChatWindow _chatWindow;
    float _inactivityTimer = 0;

    public override void OnGameStart()
    {
        if (ChatSystem.instance != null)
        {
            ChatSystem.instance.onNewLine += OnNewLine;
            _chatWindow = _chatWindowPrefab.DuplicateGO();
            _chatWindow.displayed = false;
            _chatWindow.focused = false;
        }
    }

    void OnNewLine(ChatLine line)
    {
        _chatWindow.AddLine(line);
        ResetInactivityTimer();
    }

    public override void OnGameUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_chatWindow.focused)
            {
                _chatWindow.focused = false;
                _chatWindow.ResetScroll();

                if (_chatWindow.inputText.IsNullOrEmpty() == false)
                {
                    ChatSystem.instance.SubmitMessage(_chatWindow.inputText);
                    _chatWindow.inputText = string.Empty;
                }
            }
            else
            {
                _chatWindow.displayed = true;
                _chatWindow.focused = true;
            }
        }

        if (_chatWindow.focused)
        {
            ResetInactivityTimer();
        }

        UpdateInactivityTimer();
    }

    void UpdateInactivityTimer()
    {
        _inactivityTimer -= Time.deltaTime;

        if (_inactivityTimer < 0)
        {
            if (_chatWindow.displayed)
            {
                _chatWindow.displayed = false;
            }
        }
        else
        {
            if (_chatWindow.displayed == false)
            {
                _chatWindow.displayed = true;
            }
        }
    }

    void ResetInactivityTimer()
    {
        _inactivityTimer = inactivityCloseDelay;
    }
}
