using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSessionCreation : MonoBehaviour
{
    [Header("Create")]
    [SerializeField] Button _createButton;
    [SerializeField] TMPro.TMP_InputField _sessionNameInputField;
    [SerializeField] SceneInfo _gameScene;

    [Header("Return")]
    [SerializeField] Button _returnButton;
    [SerializeField] SceneInfo _onlineRoleChoiceScene;


    OnlineServerInterface _serverInterface;
    bool _creatingSession;

    void Start()
    {
        _returnButton.onClick.AddListener(OnClick_Return);
        _createButton.onClick.AddListener(OnClick_Create);
        _sessionNameInputField.Select();
    }

    void Update()
    {
        if (_serverInterface == null)
        {
            FetchServerInterface();
        }

        _sessionNameInputField.interactable = _serverInterface != null;

        _createButton.interactable = 
            _serverInterface != null && 
            _serverInterface.isCreatingSession == false && 
            _sessionNameInputField.text.Length > 0;
    }

    void OnDestroy()
    {
        ClearServerInterface();
        WaitSpinnerService.Disable(this);
    }

    void FetchServerInterface()
    {
        _serverInterface = OnlineService.serverInterface;

        if (_serverInterface != null)
        {
            _serverInterface.onTerminate += OnOnlineInterfaceTerminated;
        }
    }

    void ClearServerInterface()
    {
        if (_serverInterface != null)
        {
            _serverInterface.onTerminate -= OnOnlineInterfaceTerminated;
            _serverInterface = null;
        }
    }

    void OnOnlineInterfaceTerminated()
    {
        ClearServerInterface();

        if (ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            DebugScreenMessage.DisplayMessage("The online interface was terminated. Check your connection.");
        }
    }

    void OnClick_Return()
    {
        ClearServerInterface();
        OnlineService.RequestRole(OnlineRole.None); // close online connection
        SceneService.Load(_onlineRoleChoiceScene);

    }

    void OnClick_Create()
    {
        if (_serverInterface.isCreatingSession == false)
        {
            WaitSpinnerService.Enable(this);
            _serverInterface.CreateSession(_sessionNameInputField.text, OnSessionCreationComplete);
        }
    }

    void OnSessionCreationComplete(bool success, string message)
    {
        WaitSpinnerService.Disable(this);

        if (success)
        {
            ClearServerInterface();
            SceneService.Load(_gameScene);
        }
        else
        {
            DebugScreenMessage.DisplayMessage("Failed to create session: " + message);
        }
    }
}
