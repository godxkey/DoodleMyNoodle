using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public float Progress { get; private set; } = 0;
        public TransferState CurrentState { get; private set; }
        public bool WasCancelledBySource { get; private set; }
        public int DataSize => _transferHeader.DataSize;
        public string Description => _transferHeader.Description;
        public Action<ReceiveViaStreamChannelOperation, byte[]> OnDataReceived;

        private ulong _streamID = ulong.MaxValue;
        private float _nextProgressUpdateTime = 0;

        private const float PROGRESS_UPDATE_INTERVAL = 2f; // update the uploader on the progress every 2s

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
                if (s_ongoingOperations.ContainsKey(_connection))
                {
                    TerminateWithAbnormalFailure("We're already receiving a LargeDataTransfer from this source.");
                    yield break;
                }

                s_ongoingOperations.Add(_connection, this);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Hook into transfer events
                ////////////////////////////////////////////////////////////////////////////////////////
                _sessionInterface.NetworkInterface.StreamDataStarted += OnStreamDataStarted;
                _sessionInterface.NetworkInterface.StreamDataProgress += OnStreamDataProgress;
                _sessionInterface.NetworkInterface.StreamDataAborted += OnStreamDataAborted;
                _sessionInterface.NetworkInterface.StreamDataReceived += OnStreamDataReceived;
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send 'Ready'
                ////////////////////////////////////////////////////////////////////////////////////////

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
                CurrentState = TransferState.WaitingForCompletedStreamData;

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

            TerminateWithSuccess();
        }

        private void OnStreamDataStarted(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID)
        {
            // if our id is unset and the connection and channel match, associate streamID
            if (connection == _connection && channel.Type == StreamChannelType.LargeDataTransfer && _streamID == ulong.MaxValue)
            {
                _streamID = streamID;
            }
        }

        private void OnStreamDataProgress(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID, float progress)
        {
            if (streamID == _streamID)
            {
                Progress = progress;

                // send a message back to the uploader with the 'progress' of the large data transfer (needed because of a Bolt api limitation ...)
                if (_nextProgressUpdateTime < Time.time)
                {
                    _nextProgressUpdateTime = Time.time + PROGRESS_UPDATE_INTERVAL;

                    _sessionInterface.SendNetMessage(new NetMessageViaStreamUpdate() { Progress = progress }, connection);
                }
            }
        }

        private void OnStreamDataAborted(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID)
        {
            if (streamID == _streamID)
            {
                WasCancelledByDestination = true;
                TerminateWithNormalFailure("StreamData aborted");
            }
        }

        private void OnStreamDataReceived(byte[] data, IStreamChannel streamChannel, INetworkInterfaceConnection source)
        {
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
            {
                _sessionInterface.NetworkInterface.StreamDataStarted -= OnStreamDataStarted;
                _sessionInterface.NetworkInterface.StreamDataProgress -= OnStreamDataProgress;
                _sessionInterface.NetworkInterface.StreamDataAborted -= OnStreamDataAborted;
                _sessionInterface.NetworkInterface.StreamDataReceived -= OnStreamDataReceived;
            }

            // remove self from 's_ongoingOperations'
            if (s_ongoingOperations.TryGetValue(_connection, out CoroutineOperation op) && op == this)
            {
                s_ongoingOperations.Remove(_connection);
            }
        }
    }
}