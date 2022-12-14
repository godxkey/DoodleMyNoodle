using CCC.Online.DataTransfer;
using CCC.Operations;
using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

// LDT: Large Data Transfer
public class SimpleClientAndServerLDTViaPackets : GlobalEventListener
{
    public Slider ProgressSlider;
    public Text DataToSend;
    public Text DataReceived;
    public Text DataMultiplierText;
    public Text DataSizeDisplay;

    public Button SendButton;
    public Button CancelButton;

    private int _dataMultiplier;
    private DirtyRef<SessionInterface> _sessionInterface;

    CoroutineOperation _op;

    private void Start()
    {
        CancelButton.onClick.AddListener(Cancel);
        SendButton.onClick.AddListener(SendLargeData);
    }

    private void Update()
    {
        _sessionInterface.Set(OnlineService.OnlineInterface?.SessionInterface);

        if (_sessionInterface.ClearDirty())
        {
            if(_sessionInterface.Get() != null)
            {
                _sessionInterface.Get().RegisterNetMessageReceiver<NetMessageChatMessage>(ReceiveLargeData);
            }
        }


        SendButton.interactable =
            _sessionInterface.Get() != null &&
            _sessionInterface.Get().Connections.Count > 0;

        CancelButton.interactable = _op != null && _op.IsRunning;


        int.TryParse(DataMultiplierText.text, out _dataMultiplier);
        int byteCount = DataToSend.text.Length * sizeof(char) * _dataMultiplier;
        DataSizeDisplay.text = $"{byteCount} bytes ({round(byteCount / (1024f * 1024f), 0.01f)} MB)";

        if (_op != null)
        {
            if (_op.IsRunning)
            {
                if (_op is SendViaManualPacketsOperation sendOp)
                {
                    UpdateDisplay(sendOp.Progress, Color.green);
                }
                else if (_op is ReceiveViaManualPacketsOperation receiveOp)
                {
                    UpdateDisplay(receiveOp.Progress, Color.green);
                }
            }
            else if (_op.HasSucceeded)
            {
                UpdateDisplay(1, Color.blue);
            }
            else
            {
                UpdateDisplay(0, Color.red);
            }
        }
        else
        {
            UpdateDisplay(0, Color.gray);
        }
    }

    string GetDataToSend()
    {
        string t = DataToSend.text;
        char[] d = new char[t.Length * _dataMultiplier];

        for (int i = 0; i < _dataMultiplier; i++)
        {
            for (int j = 0; j < t.Length; j++)
            {
                d[j + i * t.Length] = t[j];
            }
        }

        return new string(d);
    }

    void UpdateDisplay(float progress, Color color)
    {
        ProgressSlider.normalizedValue = progress;
        var c = ProgressSlider.colors;
        c.normalColor = color;
        ProgressSlider.colors = c;
    }

    private void Cancel()
    {
        _op.LogFlags = CoroutineOperation.LogFlag.None;
        _op.TerminateWithAbnormalFailure("Cancelled");
    }

    public void SendLargeData()
    {
        string dataToSend = GetDataToSend();

        foreach (var connection in OnlineService.OnlineInterface.SessionInterface.Connections)
        {
            _op = OnlineService.OnlineInterface.SessionInterface.BeginLargeDataTransfer(
                new NetMessageChatMessage() { message = dataToSend },
                connection,
                description: "A chat message");
        }
    }

    public void ReceiveLargeData(NetMessageChatMessage netMessage, INetworkInterfaceConnection source)
    {
        if (netMessage.message.Length < 100)
            DebugScreenMessage.DisplayMessage(netMessage.message);
        else
            DebugScreenMessage.DisplayMessage(netMessage.message.Length.ToString());
    }

    public override void BoltStartBegin()
    {
        base.BoltStartBegin();

        BoltNetwork.CreateStreamChannel("pogo", UdpChannelMode.Reliable, 1);
    }

    public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
    {
        base.StreamDataReceived(connection, data);

        Debug.Log($"received {data.Data.Length} from {data.Channel}");
        //DebugScreenMessage.DisplayMessage($"received {data.Data.Length} from {data.Channel}");
    }
}
