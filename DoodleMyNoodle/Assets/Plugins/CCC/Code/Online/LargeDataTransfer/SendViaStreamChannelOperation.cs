using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;

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
                var readyResponse = DisposeOnTerminate(new AwaitNetMessage<NetMessageViaStreamReady>(_sessionInterface));

                CurrentState = TransferState.WaitingForReady;
                while (readyResponse.Source != _connection || readyResponse.Response.TransferId != _transferId)
                {
                    yield return readyResponse.WaitForResponse();
                }

            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Send data
                ////////////////////////////////////////////////////////////////////////////////////////
                CurrentState = TransferState.SendingData;
                var streamChannel = _sessionInterface.NetworkInterface.GetStreamChannel(StreamChannelType.LargeDataTransfer);

                _connection.StreamBytes(streamChannel, _data);

                // listen for progress update
                _sessionInterface.RegisterNetMessageReceiver<NetMessageViaStreamUpdate>(OnProgressUpdate);
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////
                //      Await reponse from the destination that indicates that it received all of the data
                ////////////////////////////////////////////////////////////////////////////////////////
                var ackResponse = DisposeOnTerminate(new AwaitNetMessage<NetMessageViaStreamACK>(_sessionInterface));

                CurrentState = TransferState.WaitingCompleteDataACK;
                while (ackResponse.Source != _connection || ackResponse.Response.TransferId != _transferId)
                {
                    yield return ackResponse.WaitForResponse();
                }
            }

            TerminateWithSuccess();
        }

        private void OnProgressUpdate(NetMessageViaStreamUpdate message, INetworkInterfaceConnection connection)
        {
            if(connection == _connection)
            {
                Progress = message.Progress;
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

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