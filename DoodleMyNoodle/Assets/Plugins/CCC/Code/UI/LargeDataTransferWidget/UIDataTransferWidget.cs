using CCC.Online.DataTransfer;
using CCC.Operations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDataTransferWidget : MonoBehaviour
{
    public UIDataTransferWidgetElement ElementPrefab;
    public Transform ElementsSpawnContainer;
    public AudioPlayable NewTransferSFX;

    SessionInterface _session;
    List<UIDataTransferWidgetElement> _elements = new List<UIDataTransferWidgetElement>();

    void Update()
    {
        if (!IsHooked)
        {
            TryHook();
        }

        int elementIterator = 0;

        UIDataTransferWidgetElement GetOrAddWidget()
        {
            return this.GetOrAddWidget(elementIterator++);
        }

        if (IsHooked)
        {
            var incoming = _session.IncomingDataTransfers;
            var outgoing = _session.OutgoingDataTransfer;


            for (int i = 0; i < incoming.Count; i++)
            {
                if(incoming[i] is ReceiveViaManualPacketsOperation receiveViaPackets)
                {
                    var widget = GetOrAddWidget();

                    widget.Description.Set(receiveViaPackets.Description);
                    widget.TotalDataSize.Set(receiveViaPackets.DataSize);
                    widget.CurrentDataSize.Set(Mathf.FloorToInt(receiveViaPackets.Progress * receiveViaPackets.DataSize));
                    widget.IsIncoming.Set(true);
                    widget.UpdateDisplay();
                }
                else if(incoming[i] is ReceiveViaStreamChannelOperation receiveViaStream)
                {
                    var widget = GetOrAddWidget();

                    widget.Description.Set(receiveViaStream.Description);
                    widget.TotalDataSize.Set(receiveViaStream.DataSize);
                    widget.CurrentDataSize.Set(-1);
                    widget.State.Set(StateToString(receiveViaStream.CurrentState));
                    widget.IsIncoming.Set(true);
                    widget.UpdateDisplay();
                }
            }

            for (int i = 0; i < outgoing.Count; i++)
            {
                if (outgoing[i] is SendViaManualPacketsOperation sendViaPackets)
                {
                    var widget = GetOrAddWidget();

                    widget.Description.Set(sendViaPackets.Description);
                    widget.TotalDataSize.Set(sendViaPackets.DataSize);
                    widget.CurrentDataSize.Set(Mathf.FloorToInt(sendViaPackets.Progress * sendViaPackets.DataSize));
                    widget.IsIncoming.Set(false);
                    widget.UpdateDisplay();
                }
                else if(outgoing[i] is SendViaStreamChannelOperation sendViaStream)
                {
                    var widget = GetOrAddWidget();

                    widget.Description.Set(sendViaStream.Description);
                    widget.TotalDataSize.Set(sendViaStream.DataSize);
                    widget.CurrentDataSize.Set(-1);
                    widget.State.Set(StateToString(sendViaStream.CurrentState));
                    widget.IsIncoming.Set(false);
                    widget.UpdateDisplay();
                }
            }

        }
        
        DeactivateElementsFrom(elementIterator);
    }

    private string StateToString(ReceiveViaStreamChannelOperation.TransferState state)
    {
        switch (state)
        {
            default:
                return "Invalid state";

            case ReceiveViaStreamChannelOperation.TransferState.NotStarted:
                return "Not started";

            case ReceiveViaStreamChannelOperation.TransferState.SendingReady:
                return "Starting transfer";

            case ReceiveViaStreamChannelOperation.TransferState.WaitingForCompletedStreamData:
                return "Receiving data...";
            
            case ReceiveViaStreamChannelOperation.TransferState.Terminated:
                return "Transfer terminated";
        }
    }

    private string StateToString(SendViaStreamChannelOperation.TransferState state)
    {
        switch (state)
        {
            default:
                return "Invalid state";

            case SendViaStreamChannelOperation.TransferState.NotStarted:
                return "Not started";

            case SendViaStreamChannelOperation.TransferState.WaitingForStreamToBeAvailable:
                return "Waiting for available stream";

            case SendViaStreamChannelOperation.TransferState.SendingHeader:
            case SendViaStreamChannelOperation.TransferState.WaitingForReady:
                return "Starting transfer";

            case SendViaStreamChannelOperation.TransferState.SendingData:
            case SendViaStreamChannelOperation.TransferState.WaitingCompleteDataACK:
                return "Transfering data...";

            case SendViaStreamChannelOperation.TransferState.Terminated:
                return "Tranfer terminated";
        }
    }

    private void OnDisable()
    {
        Unhook();
    }

    UIDataTransferWidgetElement GetOrAddWidget(int i)
    {
        while (i >= _elements.Count)
        {
            _elements.Add(Instantiate(ElementPrefab, ElementsSpawnContainer).GetComponent<UIDataTransferWidgetElement>());
        }

        var element = _elements[i];

        // active element if necessary
        if (!element.gameObject.activeSelf)
            element.gameObject.SetActive(true);

        return element;
    }

    void DeactivateElementsFrom(int i)
    {
        for (; i < _elements.Count; i++)
        {
            if (_elements[i].gameObject.activeSelf)
                _elements[i].gameObject.SetActive(false);
        }
    }

    bool IsHooked => _session != null;

    bool TryHook()
    {
        var online = OnlineService.OnlineInterface;
        if (online != null)
        {
            var session = online.SessionInterface;
            if (session != null)
            {
                session.OnBeginReceiveLargeDataTransfer += OnBeginReceiveLargeDataTransfer;
                session.OnTerminate += Unhook;
                _session = session;
                return true;
            }
        }
        return false;
    }

    void Unhook()
    {
        if (_session != null)
        {
            _session.OnTerminate -= Unhook;
            _session.OnBeginReceiveLargeDataTransfer -= OnBeginReceiveLargeDataTransfer;
            _session = null;
        }
    }

    private void OnBeginReceiveLargeDataTransfer(CoroutineOperation arg1, INetworkInterfaceConnection arg2)
    {
        DefaultAudioSourceService.Instance.PlayStaticSFX(NewTransferSFX);
    }
}
