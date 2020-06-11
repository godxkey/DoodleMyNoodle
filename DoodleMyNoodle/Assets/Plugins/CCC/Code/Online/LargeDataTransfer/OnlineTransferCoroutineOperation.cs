using UnityEngine;

namespace CCC.Online.DataTransfer
{
    public abstract class OnlineTransferCoroutineOperation : OnlineComCoroutineOperation
    {
        protected readonly ushort _transferId;

        public bool WasCancelledByDestination { get; protected set; }

        protected OnlineTransferCoroutineOperation(SessionInterface sessionInterface, INetworkInterfaceConnection destination, ushort transferId)
            : base(sessionInterface, destination)
        {
            _transferId = transferId;
        }

        protected override bool PreExecuteRoutine()
        {
            _sessionInterface.RegisterNetMessageReceiver<NetMessageCancel>(OnTransferCancel);

            return base.PreExecuteRoutine();
        }

        private void OnTransferCancel(NetMessageCancel message, INetworkInterfaceConnection source)
        {
            if (message.TransferId == _transferId && source == _connection)
            {
                WasCancelledByDestination = true;
                TerminateWithNormalFailure("Connection cancelled the transfer");
            }
        }

        protected override void OnFail()
        {
            base.OnFail();

            if (!WasCancelledByDestination)
            {
                // notify destination we're cancelling the transfer
                if (_sessionInterface.IsConnectionValid(_connection))
                    _sessionInterface.SendNetMessage(new NetMessageCancel() { TransferId = _transferId }, _connection);
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            _sessionInterface.UnregisterNetMessageReceiver<NetMessageCancel>(OnTransferCancel);
        }
    }
}