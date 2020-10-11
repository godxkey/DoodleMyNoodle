using CCC.Operations;
using System;
using UnityEngine;
using UnityEngine.UI;
using CCC.Online.DataTransfer;
using CCC.InspectorDisplay;

public class SimpleClientAndServerConnect : MonoBehaviour
{
    public bool UseMachineNameAsRoom = true;

    [HideIf(nameof(UseMachineNameAsRoom))]
    public string RoomName;

    public Button ConnectClientButton;
    public Button DisconnectButton;
    public Button ConnectServerButton;
    public Button JoinRoomButton;

    private void Start()
    {
        ConnectClientButton.onClick.AddListener(ConnectClient);
        ConnectServerButton.onClick.AddListener(ConnectServer);
        JoinRoomButton.onClick.AddListener(() => JoinSession(UseMachineNameAsRoom ? Environment.MachineName : RoomName));
        DisconnectButton.onClick.AddListener(Disconnect);
    }

    private void Disconnect()
    {
        OnlineService.SetTargetRole(OnlineRole.None);
    }

    private void Update()
    {
        DisconnectButton.interactable = OnlineService.CurrentRole != OnlineRole.None;
        ConnectClientButton.interactable = OnlineService.CurrentRole == OnlineRole.None && !OnlineService.IsChangingRole;
        ConnectServerButton.interactable = OnlineService.CurrentRole == OnlineRole.None && !OnlineService.IsChangingRole;
        JoinRoomButton.interactable = OnlineService.OnlineInterface != null && OnlineService.OnlineInterface.SessionInterface == null
            && (OnlineService.ServerInterface != null || OnlineService.ClientInterface.AvailableSessions.Count > 0);
    }

    public void JoinSession(string sessionName)
    {
        if (OnlineService.OnlineInterface is OnlineClientInterface client)
        {
            client.ConnectToSession(client.AvailableSessions.Find((x) => x.HostName == sessionName));
        }
        else if (OnlineService.OnlineInterface is OnlineServerInterface server)
        {
            server.CreateSession(sessionName);
        }
    }

    public void ConnectClient()
    {
        OnlineService.SetTargetRole(OnlineRole.Client);
    }

    public void ConnectServer()
    {
        OnlineService.SetTargetRole(OnlineRole.Server);
    }
}
