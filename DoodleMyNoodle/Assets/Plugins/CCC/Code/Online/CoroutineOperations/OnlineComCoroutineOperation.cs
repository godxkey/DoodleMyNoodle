using CCC.Operations;
using UnityEngineX;

namespace CCC.Online.DataTransfer
{
    public abstract class OnlineComCoroutineOperation : CoroutineOperation
    {
        readonly protected SessionInterface _sessionInterface;
        readonly protected INetworkInterfaceConnection _connection;

        private bool _preExecutedCalled = false;

        protected OnlineComCoroutineOperation(SessionInterface sessionInterface, INetworkInterfaceConnection destination)
        {
            _sessionInterface = sessionInterface;
            _connection = destination;
        }

        protected virtual bool PreExecuteRoutine()
        {
            _preExecutedCalled = true;

            if (!_sessionInterface.IsConnectionValid(_connection))
            {
                return false;
            }

            _sessionInterface.OnConnectionRemoved += OnConnectionRemoved;
            _sessionInterface.OnTerminate += OnSessionTerminate;

            return true;
        }

        private void OnSessionTerminate()
        {
            TerminateWithNormalFailure("Session terminated");
        }

        private void OnConnectionRemoved(INetworkInterfaceConnection connection)
        {
            if (connection == _connection)
            {
                TerminateWithNormalFailure("Connection ended");
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            if (!_preExecutedCalled)
            {
                Log.Warning($"An operation of type {GetType().Name} has been terminated " +
                    $"without ever calling {nameof(PreExecuteRoutine)}. " +
                    $"Make sure to call the method manually in the {nameof(ExecuteRoutine)} method.");
            }

            _sessionInterface.OnConnectionRemoved -= OnConnectionRemoved;
            _sessionInterface.OnTerminate -= OnSessionTerminate;
        }
    }
}