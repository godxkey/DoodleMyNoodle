using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class FredTestScript : MonoBehaviour
{
    //public Slider ProgressSlider;
    //public Text DataToSend;
    //public Text DataReceived;
    //public Text DataMultiplierText;
    //public Text DataSizeDisplay;

    //public Button ConnectClientButton;
    //public Button DisconnectButton;
    //public Button ConnectServerButton;
    //public Button JoinRoomButton;
    //public Button CancelButton;

    //public int DataMultiplier;
    //string _json;

    //CoroutineOperation _op;

    //private void Start()
    //{
    //    ConnectClientButton.onClick.AddListener(ConnectClient);
    //    ConnectServerButton.onClick.AddListener(ConnectServer);
    //    JoinRoomButton.onClick.AddListener(() => JoinSession("toto le clooon"));
    //    DisconnectButton.onClick.AddListener(Disconnect);
    //    CancelButton.onClick.AddListener(Cancel);
    //}

    //private void Cancel()
    //{
    //    _op.LogFlags = CoroutineOperation.LogFlag.None;
    //    _op.TerminateWithFailure("Cancelled");
    //}

    //private void Disconnect()
    //{
    //    OnlineService.SetTargetRole(OnlineRole.None);
    //}

    //private void Update()
    //{
    //    int.TryParse(DataMultiplierText.text, out DataMultiplier);

    //    DisconnectButton.interactable = OnlineService.CurrentRole != OnlineRole.None;
    //    ConnectClientButton.interactable = OnlineService.CurrentRole == OnlineRole.None && !OnlineService.IsChangingRole;
    //    ConnectServerButton.interactable = OnlineService.CurrentRole == OnlineRole.None && !OnlineService.IsChangingRole;
    //    JoinRoomButton.interactable = OnlineService.OnlineInterface != null && OnlineService.OnlineInterface.SessionInterface == null
    //        && (OnlineService.ServerInterface != null || OnlineService.ClientInterface.AvailableSessions.Count > 0);
    //    CancelButton.interactable = _op != null && _op.IsRunning;

    //    DataSizeDisplay.text = "" + DataToSend.text.Length * DataMultiplier;

    //    if (_op != null)
    //    {
    //        if (_op.IsRunning)
    //        {
    //            if (_op is SendDataTransferOperation sendOp)
    //            {
    //                UpdateDisplay(sendOp.Progress, Color.green);
    //            }
    //            else if (_op is ReceiveDataTransferOperation receiveOp)
    //            {
    //                UpdateDisplay(receiveOp.Progress, Color.green);
    //            }
    //        }
    //        else if (_op.HasSucceeded)
    //        {
    //            UpdateDisplay(1, Color.blue);
    //        }
    //        else
    //        {
    //            UpdateDisplay(0, Color.red);
    //        }
    //    }
    //    else
    //    {
    //        UpdateDisplay(0, Color.gray);
    //    }
    //}

    //string GetDataToSend()
    //{
    //    string t = DataToSend.text;
    //    char[] d = new char[t.Length * DataMultiplier];

    //    for (int i = 0; i < DataMultiplier; i++)
    //    {
    //        for (int j = 0; j < t.Length; j++)
    //        {
    //            d[j + i * t.Length] = t[j];
    //        }
    //    }

    //    return new string(d);
    //}

    //INetworkInterfaceConnection GetDestination()
    //{
    //    return OnlineService.OnlineInterface.SessionInterface.Connections[0];
    //}

    //void UpdateDisplay(float progress, Color color)
    //{
    //    ProgressSlider.normalizedValue = progress;
    //    var c = ProgressSlider.colors;
    //    c.normalColor = color;
    //    ProgressSlider.colors = c;
    //}

    //public void SendLargeData()
    //{
    //    _op = OnlineService.OnlineInterface.SessionInterface.BeginLargeDataTransfer(
    //        new NetMessageChatMessage() { message = GetDataToSend() },
    //        GetDestination(),
    //        description: "A chat message");
    //}

    //public void ReceiveLargeData(NetMessageChatMessage netMessage, INetworkInterfaceConnection source)
    //{
    //    if (netMessage.message.Length < 100)
    //        DebugScreenMessage.DisplayMessage(netMessage.message);
    //    else
    //        DebugScreenMessage.DisplayMessage(netMessage.message.Length.ToString());
    //}

    //public void JoinSession(string sessionName)
    //{
    //    if (OnlineService.OnlineInterface is OnlineClientInterface client)
    //    {
    //        client.ConnectToSession(client.AvailableSessions.Find((x) => x.HostName == sessionName),
    //            (b, s) => client.SessionInterface.RegisterNetMessageReceiver<NetMessageChatMessage>(ReceiveLargeData));
    //    }
    //    else if (OnlineService.OnlineInterface is OnlineServerInterface server)
    //    {
    //        server.CreateSession(sessionName,
    //            (b, s) => server.SessionInterface.RegisterNetMessageReceiver<NetMessageChatMessage>(ReceiveLargeData));
    //    }
    //}

    //public void ConnectClient()
    //{
    //    OnlineService.SetTargetRole(OnlineRole.Client);
    //}

    //public void ConnectServer()
    //{
    //    OnlineService.SetTargetRole(OnlineRole.Server);
    //}
}


//public class TestSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            var entities = simEntityManager().CreateEntityQuery(typeof(FixTranslation)).ToEntityArray(Unity.Collections.Allocator.TempJob);
//            simEntityManager().Instantiate(entities[0]);

//            entities.Dispose();
//        }

//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            var entities = simEntityManager().CreateEntityQuery(typeof(FixTranslation)).ToEntityArray(Unity.Collections.Allocator.TempJob);
//            simEntityManager().DestroyEntity(entities[0]);
            
//            entities.Dispose();
//        }

//        if (Input.GetKeyDown(KeyCode.Alpha3))
//        {
//            simEntityManager().SetComponentData(GamePresentationCache.Instance.LocalPawn, new FixTranslation());
//        }

//        EntityManager simEntityManager()=> GameMonoBehaviourHelpers.SimulationWorld.EntityManager;
//    }
//}