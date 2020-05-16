using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CCC.Online.DataTransfer
{
    public class ReceiveViaStreamChannelOperation : OnlineTransferCoroutineOperation
    {
        static Dictionary<INetworkInterfaceConnection, CoroutineOperation> s_ongoingOperations = new Dictionary<INetworkInterfaceConnection, CoroutineOperation>();
        private bool _streamDataReceived;

        public byte[] ReceivedData { get; private set; }

        public enum TransferState
        {
            NotStarted,

            SendingReady,
            WaitingForCompletedStreamData,

            Terminated
        }

        readonly NetMessageViaStreamChannelHeader _transferHeader;


        // state
        public TransferState CurrentState { get; private set; }
        public bool WasCancelledBySource { get; private set; }
        public int DataSize => _transferHeader.DataSize;
        public string Description => _transferHeader.Description;
        public Action<ReceiveViaStreamChannelOperation, byte[]> OnDataReceived;

        public ReceiveViaStreamChannelOperation(NetMessageViaStreamChannelHeader transferHeader, INetworkInterfaceConnection source, SessionInterface sessionInterface)
            : base(sessionInterface, source, transferHeader.TransferId)
        {
            _transferHeader = transferHeader;
            CurrentState = TransferState.NotStarted;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            if (!PreExecuteRoutine())
                yield break;

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Cancel if stream not available
                ////////////////////////////////////////////////////////////////////////////////////////

                DebugService.Log($"[{nameof(ReceiveViaStreamChannelOperation)}] Start operation");
                // if there's already an ongoing transfer from this source, cancel
                if (s_ongoingOperations.ContainsKey(_connection))
                {
                    TerminateWithAbnormalFailure("We're already receiving a LargeDataTransfer from this source.");
                    yield break;
                }

                s_ongoingOperations.Add(_connection, this);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send 'Ready'
                ////////////////////////////////////////////////////////////////////////////////////////

                DebugService.Log($"[{nameof(ReceiveViaStreamChannelOperation)}] SendingReady");
                CurrentState = TransferState.SendingReady;

                if (!_sessionInterface.IsConnectionValid(_connection))
                {
                    TerminateWithNormalFailure("Source connection no longer appears to be valid");
                    yield break;
                }

                _sessionInterface.SendNetMessage(new NetMessageViaStreamReady() { TransferId = _transferHeader.TransferId }, _connection);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Wait for stream data
                ////////////////////////////////////////////////////////////////////////////////////////

                DebugService.Log($"[{nameof(ReceiveViaStreamChannelOperation)}] Wait for stream data");
                CurrentState = TransferState.WaitingForCompletedStreamData;
                _sessionInterface.NetworkInterface.StreamDataReceived += OnStreamDataReceived;

                while (!_streamDataReceived)
                {
                    yield return null;
                }

            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send ACK
                ////////////////////////////////////////////////////////////////////////////////////////
                if (_sessionInterface.IsConnectionValid(_connection))
                    _sessionInterface.SendNetMessage(new NetMessageViaStreamACK() { TransferId = _transferId }, _connection);
            }

            OnDataReceived?.Invoke(this, ReceivedData);

            DebugService.Log($"[{nameof(ReceiveViaStreamChannelOperation)}] TerminateWithSuccess");

            TerminateWithSuccess();
        }

        private void OnStreamDataReceived(byte[] data, IStreamChannel streamChannel, INetworkInterfaceConnection source)
        {
            DebugService.Log($"[{nameof(ReceiveViaStreamChannelOperation)}] OnStreamDataReceived");
            if (source == _connection)
            {
                _streamDataReceived = true;
                ReceivedData = data;

                if (_sessionInterface.NetworkInterface != null)
                    _sessionInterface.NetworkInterface.StreamDataReceived -= OnStreamDataReceived;

            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            CurrentState = TransferState.Terminated;

            if (_sessionInterface.NetworkInterface != null)
                _sessionInterface.NetworkInterface.StreamDataReceived -= OnStreamDataReceived;

            // remove self from 's_ongoingOperations'
            if (s_ongoingOperations.TryGetValue(_connection, out CoroutineOperation op) && op == this)
            {
                s_ongoingOperations.Remove(_connection);
            }
        }
    }
}