using CCC.Operations;
using System;
using System.Collections;
using UnityEngine;

namespace CCC.Online.DataTransfer
{
    public class ReceiveViaManualPacketsOperation : CoroutineOperation
    {
        enum PacketState : byte
        {
            Unreceived, Received
        }

        // init data
        readonly byte[] _data;
        readonly SessionInterface _sessionInterface;
        readonly INetworkInterfaceConnection _source;
        readonly NetMessageViaManualPacketsHeader _transferInfo;

        // state
        PacketState[] _paquetStates;
        int _remainingUnreceivedPaquets;

        public bool WasCancelledBySource { get; private set; }
        public Action<ReceiveViaManualPacketsOperation, byte[]> OnDataReceived;
        public string Description => _transferInfo.Description;
        public int DataSize => _transferInfo.DataSize;
        public float Progress => 1 - ((float)_remainingUnreceivedPaquets / _transferInfo.PacketCount);

        public ReceiveViaManualPacketsOperation(NetMessageViaManualPacketsHeader transferHeader, INetworkInterfaceConnection source, SessionInterface sessionInterface)
        {
            _transferInfo = transferHeader;
            if (_transferInfo.DataSize > Transfers.MAX_TRANSFER_SIZE)
                return;

            _data = new byte[_transferInfo.DataSize];
            _sessionInterface = sessionInterface;
            _paquetStates = new PacketState[_transferInfo.PacketCount];
            _source = source;
            _remainingUnreceivedPaquets = _transferInfo.PacketCount;
        }


        protected override IEnumerator ExecuteRoutine()
        {
            if (_data == null)
            {
                TerminateWithAbnormalFailure($"Transfer size({_transferInfo.DataSize}) exceeds limit of {Transfers.MAX_TRANSFER_SIZE}");
                yield break;
            }

            _sessionInterface.RegisterNetMessageReceiver<NetMessagePacket>(OnPaquetReceived);
            _sessionInterface.RegisterNetMessageReceiver<NetMessageCancel>(OnTransferCancelled);

            while (_remainingUnreceivedPaquets > 0)
            {
                if (!IsRunning)
                    yield break;
                yield return null;
            }

            OnDataReceived?.Invoke(this, _data);

            TerminateWithSuccess();
        }

        private void OnTransferCancelled(NetMessageCancel arg1, INetworkInterfaceConnection arg2)
        {
            WasCancelledBySource = true;
            LogFlags = LogFlag.None;
            TerminateWithAbnormalFailure("Source has cancelled the transfer");
        }

        void OnPaquetReceived(NetMessagePacket netMessage, INetworkInterfaceConnection source)
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
            if (netMessage.PacketIndex < 0 || netMessage.PacketIndex >= _paquetStates.Length)
            {
                return;
            }

            // the paquet is already acknowledged ?
            if (_paquetStates[netMessage.PacketIndex] == PacketState.Received)
            {
                return;
            }

            // mark paquet as acknowledged
            if (_paquetStates[netMessage.PacketIndex] != PacketState.Received)
            {
                _paquetStates[netMessage.PacketIndex] = PacketState.Received;
                _remainingUnreceivedPaquets--;

                // copy paquet data into _data
                int startPos = netMessage.Data.Length * netMessage.PacketIndex;
                int endPos = Mathf.Min(startPos + netMessage.Data.Length, _data.Length);

                Array.Copy(
                    sourceArray: netMessage.Data,
                    sourceIndex: 0,
                    destinationArray: _data,
                    destinationIndex: startPos,
                    length: endPos - startPos);
            }

            // notify our source that the paquet was received
            NetMessagePacketACK acknowledgeMessage = new NetMessagePacketACK()
            {
                TransferId = _transferInfo.TransferId,
                PacketIndex = netMessage.PacketIndex
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
                    _sessionInterface.SendNetMessage(new NetMessageCancel() { TransferId = _transferInfo.TransferId }, _source);
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            _sessionInterface.UnregisterNetMessageReceiver<NetMessagePacket>(OnPaquetReceived);
            _sessionInterface.UnregisterNetMessageReceiver<NetMessageCancel>(OnTransferCancelled);
        }
    }
}