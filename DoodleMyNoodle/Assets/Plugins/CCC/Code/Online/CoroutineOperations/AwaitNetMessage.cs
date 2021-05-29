using System;
using System.Collections;

namespace CCC.Online
{
    public class AwaitNetMessage<T> : IDisposable
    {
        private readonly SessionInterface _sessionInterface;
        private readonly MessagePredicate _messagePredicate;
        private bool _hasReceivedResponse;

        public delegate bool MessagePredicate(INetworkInterfaceConnection source, T response);

        public T Response = default;
        public INetworkInterfaceConnection Source = null;

        public AwaitNetMessage(SessionInterface sessionInterface, MessagePredicate messagePredicate)
        {
            _sessionInterface = sessionInterface ?? throw new ArgumentNullException(nameof(sessionInterface));
            _messagePredicate = messagePredicate ?? throw new ArgumentNullException(nameof(messagePredicate));
        }

        public IEnumerator WaitForResponse()
        {
            _hasReceivedResponse = false;
            _sessionInterface.RegisterNetMessageReceiver<T>(OnResponse);

            while (!_hasReceivedResponse)
            {
                yield return null;
            }

            _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
        }

        private void OnResponse(T arg1, INetworkInterfaceConnection arg2)
        {
            if (_messagePredicate(arg2, arg1))
            {
                _hasReceivedResponse = true;
                Response = arg1;
                Source = arg2;
            }
        }

        public void Dispose()
        {
            _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
        }
    }
}
