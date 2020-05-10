using CCC.Operations;
using System;
using System.Collections;

namespace CCC.Online.DataTransfer
{
    public class SendViaStreamChannelOperation : CoroutineOperation
    {
        [ConfigVar(name: "stream_bandwidth", defaultValue: "20", description: "The stream bandwidth used while transfering large data between server and client (in KB/s)")]
        static ConfigVar s_streamBandwidth;

        // init data
        readonly byte[] _data;
        readonly SessionInterface _sessionInterface;
        readonly INetworkInterfaceConnection _destination;
        readonly ushort _transferId;

        // state
        public string Description { get; private set; }
        public bool WasCancelledByDestination { get; private set; }
        public int DataSize => _data.Length;

        /// <summary>
        /// DO NOT MODIFY THE BYTE[] DATA WILL THE TRANSFER IS ONGOING
        /// </summary>
        public SendViaStreamChannelOperation(byte[] data, INetworkInterfaceConnection destination, SessionInterface sessionInterface, string description = "")
        {
            if (data.Length > Transfers.MAX_TRANSFER_SIZE)
                throw new Exception($"Data transfer ({data.Length} bytes) cannot exceed {Transfers.MAX_TRANSFER_SIZE} bytes.");

            _data = data;
            _sessionInterface = sessionInterface;

            _destination = destination;
            _transferId = Transfers.s_NextTransferId++;
            Description = description;
        }


        protected override IEnumerator ExecuteRoutine()
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            //      Send header to destination (contains essential details about the transfer)
            ////////////////////////////////////////////////////////////////////////////////////////
            NetMessageViaStreamChannelHeader header = new NetMessageViaStreamChannelHeader()
            {
                TransferId = _transferId,
                DataSize = _data.Length,
                Description = Description,
                ChannelName = "pogo"
            };
            _sessionInterface.SendNetMessage(header, _destination);

            // listen for cancel
            _sessionInterface.RegisterNetMessageReceiver<NetMessageCancel>(OnTransferCancelled);


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Update Transfer
            ////////////////////////////////////////////////////////////////////////////////////////
            yield return null;

            var streamChannel = _sessionInterface.NetworkInterface.GetStreamChannel(StreamChannelType.LargeDataTransfer);

            _destination.SetStreamBandwidth(1024 * s_streamBandwidth.IntValue);
            _destination.StreamBytes(streamChannel, _data);



            TerminateWithSuccess();
        }

        private void OnTransferCancelled(NetMessageCancel arg1, INetworkInterfaceConnection arg2)
        {
            WasCancelledByDestination = true;
            LogFlags = LogFlag.None;
            TerminateWithFailure("Destination has cancelled the transfer");
        }

        protected override void OnFail()
        {
            base.OnFail();

            if (!WasCancelledByDestination)
            {
                // notify destination we're cancelling the transfer
                if (_sessionInterface.IsConnectionValid(_destination))
                    _sessionInterface.SendNetMessage(new NetMessageCancel() { TransferId = _transferId }, _destination);
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            _sessionInterface.UnregisterNetMessageReceiver<NetMessageCancel>(OnTransferCancelled);
        }
    }
}