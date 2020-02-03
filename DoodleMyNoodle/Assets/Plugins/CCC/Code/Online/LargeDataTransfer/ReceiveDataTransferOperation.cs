using CCC.Operations;
using System;
using System.Collections;
using UnityEngine;

public class ReceiveDataTransferOperation : CoroutineOperation
{
    enum PaquetState : byte
    {
        Unreceived, Received
    }

    // init data
    readonly byte[] _data;
    readonly SessionInterface _sessionInterface;
    readonly INetworkInterfaceConnection _source;
    readonly NetMessageDataTransferHeader _transferInfo;

    // state
    PaquetState[] _paquetStates;
    int _remainingUnreceivedPaquets;

    public bool WasCancelledBySource { get; private set; }
    public Action<ReceiveDataTransferOperation, byte[]> OnDataReceived;
    public string Description => _transferInfo.Description;
    public int DataSize => _transferInfo.DataSize;
    public float Progress => 1 - ((float)_remainingUnreceivedPaquets / _transferInfo.PaquetCount);

    /// <summary>
    /// DO NOT MODIFY THE BYTE[] DATA WILL THE TRANSFER IS ONGOING
    /// </summary>
    public ReceiveDataTransferOperation(NetMessageDataTransferHeader transferHeader, INetworkInterfaceConnection source, SessionInterface sessionInterface)
    {
        _transferInfo = transferHeader;
        if (_transferInfo.DataSize > DataTransferConstants.MAX_TRANSFER_SIZE)
            return;

        _data = new byte[_transferInfo.DataSize];
        _sessionInterface = sessionInterface;
        _paquetStates = new PaquetState[_transferInfo.PaquetCount];
        _source = source;
        _remainingUnreceivedPaquets = _transferInfo.PaquetCount;
    }


    protected override IEnumerator ExecuteRoutine()
    {
        if (_data == null)
        {
            TerminateWithFailure($"Transfer size({_transferInfo.DataSize}) exceeds limit of {DataTransferConstants.MAX_TRANSFER_SIZE}");
            yield break;
        }

        _sessionInterface.RegisterNetMessageReceiver<NetMessageDataTransferPaquet>(OnPaquetReceived);
        _sessionInterface.RegisterNetMessageReceiver<NetMessageDataTransferCancel>(OnTransferCancelled);

        while (_remainingUnreceivedPaquets > 0)
        {
            if (!IsRunning)
                yield break;
            yield return null;
        }

        OnDataReceived?.Invoke(this, _data);

        TerminateWithSuccess();
    }

    private void OnTransferCancelled(NetMessageDataTransferCancel arg1, INetworkInterfaceConnection arg2)
    {
        WasCancelledBySource = true;
        LogFlags = LogFlag.None;
        TerminateWithFailure("Source has cancelled the transfer");
    }

    void OnPaquetReceived(NetMessageDataTransferPaquet netMessage, INetworkInterfaceConnection source)
    {
        // Our source has sent us a paquet!

        // the message comes from our source ?
        if (source != _source)
        {
            return;
        }

        // the message is about our transfer ? (we could have many in parallel)
        if (netMessage.TransferId != _transferInfo.TransferId)
        {
            return;
        }

        // the paquet index is valid ?
        if (netMessage.PaquetIndex < 0 || netMessage.PaquetIndex >= _paquetStates.Length)
        {
            return;
        }

        // the paquet is already acknowledged ?
        if (_paquetStates[netMessage.PaquetIndex] == PaquetState.Received)
        {
            return;
        }

        // mark paquet as acknowledged
        if (_paquetStates[netMessage.PaquetIndex] != PaquetState.Received)
        {
            _paquetStates[netMessage.PaquetIndex] = PaquetState.Received;
            _remainingUnreceivedPaquets--;

            // copy paquet data into _data
            int startPos = netMessage.Data.Length * netMessage.PaquetIndex;
            int endPos = Mathf.Min(startPos + netMessage.Data.Length, _data.Length);

            Array.Copy(
                sourceArray: netMessage.Data,
                sourceIndex: 0,
                destinationArray: _data,
                destinationIndex: startPos,
                length: endPos - startPos);
        }

        // notify our source that the paquet was received
        NetMessageDataTransferPaquetACK acknowledgeMessage = new NetMessageDataTransferPaquetACK()
        {
            TransferId = _transferInfo.TransferId,
            PaquetIndex = netMessage.PaquetIndex
        };

        _sessionInterface.SendNetMessage(acknowledgeMessage, source);
    }

    protected override void OnFail()
    {
        base.OnFail();

        if (!WasCancelledBySource)
        {
            // notify source we're cancelling the transfer
            if (_sessionInterface.IsConnectionValid(_source))
                _sessionInterface.SendNetMessage(new NetMessageDataTransferCancel() { TransferId = _transferInfo.TransferId }, _source);
        }
    }

    protected override void OnTerminate()
    {
        base.OnTerminate();

        _sessionInterface.UnregisterNetMessageReceiver<NetMessageDataTransferPaquet>(OnPaquetReceived);
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageDataTransferCancel>(OnTransferCancelled);
    }
}