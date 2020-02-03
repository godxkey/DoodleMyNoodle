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

    [Header("Return")]
    [SerializeField] Button _returnButton;


    void Start()
    {
        _sessionNameInputField.text = Environment.UserName + "'s server";

        _returnButton.onClick.AddListener(OnClick_Return);
        _createButton.onClick.AddListener(OnClick_Create);
        _sessionNameInputField.Select();
    }

    void Update()
    {
        OnlineServerInterface serverInterface = OnlineService.ServerInterface;

        _sessionNameInputField.interactable = serverInterface != null;

        _createButton.interactable =
            serverInterface != null &&
            serverInterface.isCreatingSession == false &&
            _sessionNameInputField.text.Length > 0;
    }

    void OnClick_Return()
    {
        ((GameStateLobbyServer)GameStateManager.currentGameState).Return();
    }

    void OnClick_Create()
    {
        ((GameStateLobbyServer)GameStateManager.currentGameState).CreateSession(_sessionNameInputField.text, null);
    }
}
