using CCC.Operations;
using System;
using System.Collections;
using UnityEngine;


public class SendDataTransferOperation : CoroutineOperation
{
    enum PaquetState : byte
    {
        Unsent, Unacknowledged, Acknowledged
    }

    // static fields (shared across data transfer operations)
    static int s_paquetsSentThisFrame;
    static AutoResetDirtyValue<int> s_currentUnityFrame;
    static byte[] s_cachedPaquetData = new byte[DataTransferConstants.PAQUET_BYTE_ARRAY_SIZE];
    static ushort s_nextTransferId = 0;

    // init data
    readonly byte[] _data;
    readonly SessionInterface _sessionInterface;
    readonly INetworkInterfaceConnection _destination;
    readonly ushort _transferId;

    // state
    PaquetState[] _paquetStates;
    int _paquetIterator;
    int _remainingUnacknowledgedPaquets;
    

    public string Description { get; private set; }
    public bool WasCancelledByDestination { get; private set; }
    public int DataSize => _data.Length;
    public float Progress => 1 - ((float)_remainingUnacknowledgedPaquets / _paquetStates.Length);

    /// <summary>
    /// DO NOT MODIFY THE BYTE[] DATA WILL THE TRANSFER IS ONGOING
    /// </summary>
    public SendDataTransferOperation(byte[] data, INetworkInterfaceConnection destination, SessionInterface sessionInterface, string description = "")
    {
        if (data.Length > DataTransferConstants.MAX_TRANSFER_SIZE)
            throw new Exception($"Data transfer ({data.Length} bytes) cannot exceed {DataTransferConstants.MAX_TRANSFER_SIZE} bytes.");

        _data = data;
        _sessionInterface = sessionInterface;
        int totalPaquetCount = data.Length / DataTransferConstants.PAQUET_BYTE_ARRAY_SIZE;
        if (data.Length % DataTransferConstants.PAQUET_BYTE_ARRAY_SIZE != 0)
            totalPaquetCount++;

        _paquetStates = new PaquetState[totalPaquetCount];
        _destination = destination;
        _remainingUnacknowledgedPaquets = totalPaquetCount;
        _transferId = s_nextTransferId++;
        Description = description;
    }


    protected override IEnumerator ExecuteRoutine()
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Send header to destination (contains essential details about the transfer)
        ////////////////////////////////////////////////////////////////////////////////////////
        NetMessageDataTransferHeader header = new NetMessageDataTransferHeader()
        {
            TransferId = _transferId,
            DataSize = _data.Length,
            PaquetCount = _remainingUnacknowledgedPaquets,
            Description = Description
        };
        _sessionInterface.SendNetMessage(header, _destination);


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Update Transfer
        ////////////////////////////////////////////////////////////////////////////////////////
        _sessionInterface.RegisterNetMessageReceiver<NetMessageDataTransferPaquetACK>(OnPaquetAcknowledged);
        _sessionInterface.RegisterNetMessageReceiver<NetMessageDataTransferCancel>(OnTransferCancelled);

        while (_remainingUnacknowledgedPaquets > 0)
        {
            // reset 'paquetsSentThisFrame'. This value will ensure we don't send to many paquets per second to our sessionInterface
            s_currentUnityFrame.SetValue(Time.frameCount);
            if (s_currentUnityFrame.IsDirty)
            {
                s_paquetsSentThisFrame = 0;
            }

            // Update
            UpdateDataTransfer();

            // wait a frame
            yield return null;

            if (!IsRunning)
            {
                yield break;
            }
        }

        TerminateWithSuccess();
    }

    private void OnTransferCancelled(NetMessageDataTransferCancel arg1, INetworkInterfaceConnection arg2)
    {
        WasCancelledByDestination = true;
        LogFlags = LogFlag.None;
        TerminateWithFailure("Destination has cancelled the transfer");
    }

    void UpdateDataTransfer()
    {
        if (_sessionInterface.IsConnectionValid(_destination))
        {
            if (s_paquetsSentThisFrame < DataTransferConstants.MAX_SENT_PAQUETS_PER_FRAME)
            {
                for (int i = 0; i < _paquetStates.Length; i++)
                {
                    _paquetIterator++;
                    if (_paquetIterator >= _paquetStates.Length)
                        _paquetIterator = 0;

                    if (_paquetStates[_paquetIterator] != PaquetState.Acknowledged)
                    {
                        _paquetStates[_paquetIterator] = PaquetState.Unacknowledged;
                        SendPaquet(_destination, _paquetIterator);

                        s_paquetsSentThisFrame++;
                        if (s_paquetsSentThisFrame >= DataTransferConstants.MAX_SENT_PAQUETS_PER_FRAME)
                            break;
                    }
                }
            }
        }
        else
        {
            TerminateWithFailure("Connection to destination is no longer valid.");
        }
    }

    void SendPaquet(INetworkInterfaceConnection destination, int paquetIndex)
    {
        // create message
        NetMessageDataTransferPaquet netMessage = new NetMessageDataTransferPaquet()
        {
            PaquetIndex = paquetIndex,
            Data = s_cachedPaquetData,
            TransferId = _transferId
        };

        // copy slice of array into message data
        int byteIndex = paquetIndex * DataTransferConstants.PAQUET_BYTE_ARRAY_SIZE;
        System.Array.Copy(
            sourceArray: _data,
            sourceIndex: byteIndex,
            destinationArray: netMessage.Data,
            destinationIndex: 0,
            length: Mathf.Min(_data.Length - byteIndex, DataTransferConstants.PAQUET_BYTE_ARRAY_SIZE));

        // send message
        _sessionInterface.SendNetMessage(netMessage, destination, reliableAndOrdered: false);
    }


    void OnPaquetAcknowledged(NetMessageDataTransferPaquetACK netMessage, INetworkInterfaceConnection source)
    {
        // Our destination has confirmed a paquet receival!

        // the message comes from our target ?
        if (source != _destination)
        {
            return;
        }

        // the message is about our current transfer (we could have more than one in parallel)
        if (netMessage.TransferId != _transferId)
        {
            return;
        }

        // the paquet index is valid ?
        if (netMessage.PaquetIndex < 0 || netMessage.PaquetIndex >= _paquetStates.Length)
        {
            return;
        }

        // the paquet is already acknowledged ?
        if (_paquetStates[netMessage.PaquetIndex] == PaquetState.Acknowledged)
        {
            return;
        }

        // mark paquet as acknowledged
        _paquetStates[netMessage.PaquetIndex] = PaquetState.Acknowledged;
        _remainingUnacknowledgedPaquets--;

        // data transfer complete ?
        if (_remainingUnacknowledgedPaquets == 0)
        {
            TerminateWithSuccess();
        }
    }

    protected override void OnFail()
    {
        base.OnFail();

        if (!WasCancelledByDestination)
        {
            // notify destination we're cancelling the transfer
            if (_sessionInterface.IsConnectionValid(_destination))
                _sessionInterface.SendNetMessage(new NetMessageDataTransferCancel() { TransferId = _transferId }, _destination);
        }
    }

    protected override void OnTerminate()
    {
        base.OnTerminate();

        _sessionInterface.UnregisterNetMessageReceiver<NetMessageDataTransferPaquetACK>(OnPaquetAcknowledged);
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageDataTransferCancel>(OnTransferCancelled);
    }
}