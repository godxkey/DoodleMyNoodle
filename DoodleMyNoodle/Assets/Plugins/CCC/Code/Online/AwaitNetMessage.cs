using System;
using System.Collections;

namespace CCC.Online
{
    public class AwaitNetMessage<T> : IDisposable
    {
        private SessionInterface _sessionInterface;
        private bool _hasReceivedResponse;

        public T Response = default;
        public INetworkInterfaceConnection Source = null;

        public AwaitNetMessage(SessionInterface sessionInterface)
        {
            _sessionInterface = sessionInterface ?? throw new ArgumentNullException(nameof(sessionInterface));
        }

        public IEnumerator WaitForResponse()
        {
            _hasReceivedResponse = false;
            _sessionInterface.RegisterNetMessageReceiver<T>(OnResponse);
            DebugService.Log($"RegisterNetMessageReceiver<{typeof(T).Name}>");
            while (!_hasReceivedResponse)
            {
                DebugService.Log($"yield return null <{typeof(T).Name}>");
                yield return null;
            }

            DebugService.Log($"UnregisterNetMessageReceiver<{typeof(T).Name}>");
            _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
        }

        private void OnResponse(T arg1, INetworkInterfaceConnection arg2)
        {
            DebugService.Log($"OnResponse<{typeof(T).Name}>");
            _hasReceivedResponse = true;
            Response = arg1;
            Source = arg2;
        }

        public void Dispose()
        {
            _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
        }
    }
}
