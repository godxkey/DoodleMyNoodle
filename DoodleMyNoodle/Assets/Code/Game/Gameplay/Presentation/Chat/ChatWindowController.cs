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
        if (ChatSystem.Instance != null)
        {
            ChatSystem.Instance.OnNewLine += OnNewLine;
            _chatWindow = Instantiate(_chatWindowPrefab);
            _chatWindow.displayed = false;
            _chatWindow.focused = false;
        }
        else
        {
            // if we have no chat system, destroy the chat window controller (for now, this happens when playing in 'local' mode)
            Destroy(gameObject);
        }
    }

    void OnNewLine(ChatLine line)
    {
        _chatWindow.AddLine(line);
        ResetInactivityTimer();
    }

    public override void OnGameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !GameConsole.IsOpen())
        {
            if (_chatWindow.focused)
            {
                _chatWindow.focused = false;
                _chatWindow.ResetScroll();

                if (string.IsNullOrEmpty(_chatWindow.inputText) == false)
                {
                    ChatSystem.Instance.SubmitMessage(_chatWindow.inputText);
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
