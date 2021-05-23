using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngineX;

namespace CCC.Online.DataTransfer
{
    public class SendViaStreamChannelOperation : OnlineTransferCoroutineOperation
    {
        static Dictionary<INetworkInterfaceConnection, CoroutineOperation> s_ongoingOperations = new Dictionary<INetworkInterfaceConnection, CoroutineOperation>();

        public enum TransferState
        {
            NotStarted,
            WaitingForStreamToBeAvailable,
            SendingHeader,
            WaitingForReady,
            SendingData,
            WaitingCompleteDataACK,

            Terminated
        }

        // init data
        readonly byte[] _data;

        // state
        public TransferState CurrentState { get; private set; }
        public string Description { get; private set; }
        public int DataSize => _data.Length;
        public float Progress { get; private set; } = 0;

        /// <summary>
        /// DO NOT MODIFY THE BYTE[] DATA WILL THE TRANSFER IS ONGOING
        /// </summary>
        public SendViaStreamChannelOperation(byte[] data, INetworkInterfaceConnection destination, SessionInterface sessionInterface, string description = "")
            : base(sessionInterface, destination, Transfers.s_NextTransferId++)
        {
            if (data.Length > Transfers.MAX_TRANSFER_SIZE)
                throw new Exception($"Data transfer ({data.Length} bytes) cannot exceed {Transfers.MAX_TRANSFER_SIZE} bytes.");

            _data = data;
            Description = description;
            CurrentState = TransferState.NotStarted;
        }


        protected override IEnumerator ExecuteRoutine()
        {
            if (!PreExecuteRoutine())
                yield break;

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Wait for stream to be available
                ////////////////////////////////////////////////////////////////////////////////////////

                CurrentState = TransferState.WaitingForStreamToBeAvailable;
                // if there's already an ongoing transfer to this destination, wait
                while (s_ongoingOperations.ContainsKey(_connection))
                {
                    yield return null;
                }

                s_ongoingOperations.Add(_connection, this);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send header to destination (contains essential details about the transfer)
                ////////////////////////////////////////////////////////////////////////////////////////
                CurrentState = TransferState.SendingHeader;
                NetMessageViaStreamChannelHeader header = new NetMessageViaStreamChannelHeader()
                {
                    TransferId = _transferId,
                    DataSize = _data.Length,
                    Description = Description,
                };
                _sessionInterface.SendNetMessage(header, _connection);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Await the 'ready!' reponse
                ////////////////////////////////////////////////////////////////////////////////////////
                var readyResponse = DisposeOnTerminate(new AwaitNetMessage<NetMessageViaStreamReady>(_sessionInterface, (source, response) =>
                {
                    return source == _connection && response.TransferId == _transferId;
                }));

                CurrentState = TransferState.WaitingForReady;

                yield return readyResponse.WaitForResponse();
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send data
                ////////////////////////////////////////////////////////////////////////////////////////
                CurrentState = TransferState.SendingData;
                var streamChannel = _sessionInterface.NetworkInterface.GetStreamChannel(StreamChannelType.LargeDataTransfer);

                // listen for progress update
                _sessionInterface.RegisterNetMessageReceiver<NetMessageViaStreamUpdate>(OnProgressUpdate);
                _sessionInterface.NetworkInterface.StreamDataAborted += OnStreamAborted;

                _connection.StreamBytes(streamChannel, _data);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Await reponse from the destination that indicates that it received all of the data
                ////////////////////////////////////////////////////////////////////////////////////////
                var ackResponse = DisposeOnTerminate(new AwaitNetMessage<NetMessageViaStreamACK>(_sessionInterface, (source, response) =>
                {
                    return source == _connection && response.TransferId == _transferId;
                }));

                CurrentState = TransferState.WaitingCompleteDataACK;

                yield return ackResponse.WaitForResponse();
            }

            TerminateWithSuccess();
        }

        private void OnStreamAborted(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID)
        {
            if (connection == Connection && channel.Type == StreamChannelType.LargeDataTransfer)
            {
                _sessionInterface.NetworkInterface.StreamDataAborted -= OnStreamAborted;
                TerminateWithNormalFailure("Stream aborted");
            }
        }

        private void OnProgressUpdate(NetMessageViaStreamUpdate message, INetworkInterfaceConnection connection)
        {
            if (connection == _connection)
            {
                Progress = message.Progress;
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            _sessionInterface.NetworkInterface.StreamDataAborted -= OnStreamAborted;
            _sessionInterface.UnregisterNetMessageReceiver<NetMessageViaStreamUpdate>(OnProgressUpdate);
            CurrentState = TransferState.Terminated;

            // remove self from 's_ongoingOperations'
            if (s_ongoingOperations.TryGetValue(_connection, out CoroutineOperation op) && op == this)
            {
                s_ongoingOperations.Remove(_connection);
            }
        }
    }
}