using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Internals.MenuSessionChoice;
using UnityEngine.SceneManagement;

public class MenuSessionChoice : MonoBehaviour
{
    [Header("Sessions")]
    [SerializeField] RectTransform _sessionButtonContainer;
    [SerializeField] SessionButton _sessionButtonPrefab;
    [SerializeField] GameObject _spinnerIcon;
    [SerializeField] Button _refreshButton;

    [Header("Join")]
    [SerializeField] Button _joinButton;
    [SerializeField] SceneInfo _gameScene;

    [Header("Return")]
    [SerializeField] Button _returnButton;
    [SerializeField] SceneInfo _onlineRoleChoiceScene;

    List<SessionButton> _sessionButtons = new List<SessionButton>();
    List<INetworkInterfaceSession> _sessions = new List<INetworkInterfaceSession>();
    INetworkInterfaceSession _selectedSession;
    OnlineClientInterface _clientInterface;

    void Awake()
    {
        _joinButton.onClick.AddListener(OnClick_Join);
        _returnButton.onClick.AddListener(OnClick_Return);

        // thats the only thing we can do. We must wait for the NetworkInterface to notify us of changes (and we're always subscribed)
        _refreshButton.onClick.AddListener(() => _spinnerIcon.SetActive(true));
    }

    void Update()
    {
        if (_clientInterface == null)
        {
            FetchClientInterface();
        }

        _joinButton.interactable = _selectedSession != null;
    }

    void OnDestroy()
    {
        ClearClientInterface();
        WaitSpinnerService.Disable(this);
    }

    void FetchClientInterface()
    {
        _clientInterface = OnlineService.clientInterface;

        if (_clientInterface != null)
        {
            _clientInterface.onTerminate += OnOnlineInterfaceTerminated;
            _clientInterface.onSessionListUpdated += OnSessionListUpdated;
            _clientInterface.GetAvailableSessions(ref _sessions);
        }

        UpdateSessionButtonList();
    }

    void ClearClientInterface()
    {
        if (_clientInterface != null)
        {
            _clientInterface.onSessionListUpdated -= OnSessionListUpdated;
            _clientInterface.onTerminate -= OnOnlineInterfaceTerminated;
            _clientInterface = null;
        }
    }

    void OnOnlineInterfaceTerminated()
    {
        ClearClientInterface();

        if (ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            DebugScreenMessage.DisplayMessage("The online interface was terminated. Check your connection.");
        }
    }

    void OnSessionListUpdated()
    {
        _spinnerIcon.SetActive(false);
        _clientInterface?.GetAvailableSessions(ref _sessions);
        UpdateSessionButtonList();
    }

    void OnClick_Return()
    {
        ClearClientInterface();
        OnlineService.RequestRole(OnlineRole.None); // close online connection
        SceneService.Load(_onlineRoleChoiceScene);
    }

    void OnClick_Join()
    {
        if (_selectedSession != null)
        {
            OnlineService.clientInterface.ConnectToSession(_selectedSession, OnConnectingToSessionComplete);
            WaitSpinnerService.Enable(this);
        }
        else
        {
            string message = "Cannot join null session";
            DebugScreenMessage.DisplayMessage(message);
            DebugService.LogError(message);
        }
    }

    void OnConnectingToSessionComplete(bool success, string message)
    {
        WaitSpinnerService.Disable(this);
        if (success)
        {
            SceneService.Load(_gameScene);
        }
        else
        {
            DebugScreenMessage.DisplayMessage(message);
            DebugService.LogError(message);
        }
    }

    void UpdateSessionButtonList()
    {
        bool oneItemIsSelected = false;

        // update existing buttons and add new ones
        for (int i = 0; i < _sessions.Count; i++)
        {
            if (i == _sessionButtons.Count)
            {
                // create new button!
                SessionButton newButton = _sessionButtonPrefab.DuplicateGO(_sessionButtonContainer);
                newButton.onClick += OnSessionButtonClick;
                _sessionButtons.Add(newButton);
            }

            _sessionButtons[i].session = _sessions[i];
            _sessionButtons[i].selected = _selectedSession == _sessions[i];
            if (_sessionButtons[i].selected)
            {
                oneItemIsSelected = true;
            }
        }

        // remove extra buttons
        for (int r = _sessionButtons.Count - 1; r >= _sessions.Count; r--)
        {
            _sessionButtons[r].DestroyGO();
            _sessionButtons.RemoveLast();
        }

        if (oneItemIsSelected == false)
        {
            // we were focusing a session that does not exist anymore
            _selectedSession = null;
        }
    }

    private void OnSessionButtonClick(SessionButton button)
    {
        _selectedSession = button.session;
        UpdateSessionButtonList();
    }
}
