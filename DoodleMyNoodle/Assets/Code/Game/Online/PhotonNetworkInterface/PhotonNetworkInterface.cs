using Bolt;
using Bolt.photon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UdpKit;
using UnityEngine;

using OperationResultCallback = System.Action<bool, string>;

namespace Internals.PhotonNetwokInterface
{
    public class PhotonNetworkInterface : NetworkInterface
    {
        public override event Action onDisconnectedFromSession;
        public override event Action onShutdownBegin;
        public override event Action<INetworkInterfaceConnection> onDisconnect;
        public override event Action onSessionListUpdated;

        public override bool isServer => BoltNetwork.IsServer;
        public override ReadOnlyCollection<INetworkInterfaceConnection> connections { get; }
        public override INetworkInterfaceSession connectedSessionInfo => _connectedSessionInfo;
        public override void GetSessions(ref List<INetworkInterfaceSession> list)
        {
            list.Clear();
            foreach (KeyValuePair<Guid, UdpSession> session in BoltNetwork.SessionList)
            {
                list.Add(new PhotonNetworkInterfaceSession(session.Value));
            }
        }

        public PhotonNetworkInterface()
        {
            connections = _connections.AsReadOnly();
            CreateBoltListener();
        }

        public override void LaunchClient(OperationResultCallback onComplete)
        {
            _operationCallbackLaunch = onComplete;
            state = NetworkState.Launching;
            BoltLauncher.StartClient();
        }
        public override void LaunchServer(OperationResultCallback onComplete)
        {
            _operationCallbackLaunch = onComplete;
            state = NetworkState.Launching;
            BoltLauncher.StartServer();
        }

        public override void Shutdown(OperationResultCallback onComplete)
        {
            _operationCallbackShutdown = onComplete;
            state = NetworkState.ShuttingDown;
            BoltLauncher.Shutdown();
        }

        public override void CreateSession(string sessionName, OperationResultCallback onComplete)
        {
            if (state != NetworkState.Running)
            {
                DebugService.Log("[PhotonNetworkInterface] Cannot create session if not in running state");
                return;
            }

            if (!BoltNetwork.IsServer)
            {
                DebugService.Log("[PhotonNetworkInterface] Cannot create session if not of type server");
                return;
            }

            _operationCallbackSessionCreated = onComplete;

            // Create some room custom properties
            PhotonRoomProperties roomProperties = new PhotonRoomProperties();

            roomProperties.IsOpen = true;
            roomProperties.IsVisible = true;

            // Create the Photon Room
            BoltNetwork.SetServerInfo(sessionName, roomProperties);

            DebugService.Log("[PhotonNetworkInterface] Session creation begun...");
        }

        public override void ConnectToSession(INetworkInterfaceSession session, OperationResultCallback onComplete)
        {
            _operationCallbackSessionConnected = onComplete;

            UdpSession udpSession = ((PhotonNetworkInterfaceSession)session).UdpSession;

            BoltNetwork.Connect(udpSession);

            _connectedSessionInfo = session;
        }

        public override void DisconnectFromSession(OperationResultCallback onComplete)
        {
            _connectedSessionInfo = null;
            BoltNetwork.Connect(null);
        }

        public override void Update()
        {
            if (!isServer)
            {
                if (_sessionClearTimer < 0 && BoltNetwork.IsRunning)
                {
                    // clear the session list. 
                    // This is needed because we apparently don't get session updates if there are no sessions
                    BoltNetwork.UpdateSessionList(new Map<Guid, UdpSession>());
                }

                _sessionClearTimer -= Time.deltaTime;
            }

            if (state == NetworkState.ShuttingDown && !BoltNetwork.IsRunning)
            {
                state = NetworkState.Stopped;

                if (_connections.Count != 0)
                {
                    _connections.Clear();
                    DebugService.Log("[PhotonNetworkInterface] Shut down complete weirdness: m_connections.Count > 0");
                }

                if (_connectedSessionInfo != null)
                {
                    _connectedSessionInfo = null;
                    onDisconnectedFromSession?.Invoke();
                }

                ConcludeOperationCallback(ref _operationCallbackDisconnectedFromSession, true, null);
                ConcludeOperationCallback(ref _operationCallbackShutdown, true, null);
            }
        }

        public override void SendMessage(INetworkInterfaceConnection connection, byte[] data, int size)
        {
            if (connection == null)
            {
                Debug.LogError("[PhotonNetworkInterface] Cannot send message to null connection");
                return;
            }

            BoltConnection boltConnection = ((PhotonNetworkInterfaceConnection)connection).BoltConnection;
            BoltCommunicationEvent evt = BoltCommunicationEvent.Create(boltConnection, Bolt.ReliabilityModes.ReliableOrdered);
            evt.BinaryData = data;
            evt.Send();
        }

        public override void SetMessageReader(Action<INetworkInterfaceConnection, byte[], int> messageReader)
        {
            _messageReader = messageReader;
        }

        public override void Dispose()
        {
            DestroyListener();
        }

        #region Event Handling
        public void Event_BoltStartBegin()
        {
            DebugService.Log("[PhotonNetworkInterface] BoltStartBegin");
            RegisterTokenClasses();
        }

        public void Event_BoltStartDone()
        {
            state = NetworkState.Running;
            DebugService.Log("[PhotonNetworkInterface] BoltStartDone");

            _operationCallbackLaunch?.Invoke(true, null);
        }

        public void Event_BoltStartFailed()
        {
            state = NetworkState.Stopped;
            DebugService.Log("[PhotonNetworkInterface] BoltStartFailed");

            _operationCallbackLaunch?.Invoke(false, "Bolt failed to start");
        }

        public void Event_BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            state = NetworkState.ShuttingDown;
            DebugService.Log("[PhotonNetworkInterface] BoltShutdownBegin");

            onShutdownBegin?.Invoke();
        }

        public void Event_Connected(BoltConnection connection)
        {
            DebugService.Log("[PhotonNetworkInterface] Connected: " + connection.ToString());
            _connections.Add(new PhotonNetworkInterfaceConnection(connection));

            // NOTE: this event will get called for each new connection
            //     client: called once when we join the session
            //     server: called multiple times
            ConcludeOperationCallback(ref _operationCallbackSessionConnected, true, null);
        }

        public void Event_Disconnected(BoltConnection connection)
        {
            DebugService.Log("[PhotonNetworkInterface] Disconnected: " + connection.ToString());

            INetworkInterfaceConnection connectionInterface = null;

            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                if (_connections[i].Id == connection.ConnectionId)
                {
                    connectionInterface = _connections[i];
                    _connections.RemoveAt(i);
                }
            }

            onDisconnect?.Invoke(connectionInterface);

            if (isClient && _connections.Count == 0 && _connectedSessionInfo != null)
            {
                ConcludeOperationCallback(ref _operationCallbackDisconnectedFromSession, true, null);

                onDisconnectedFromSession?.Invoke();
                _connectedSessionInfo = null;
            }
        }

        public void Event_ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            DebugService.Log("[PhotonNetworkInterface] ConnectAttempt: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            DebugService.Log("[PhotonNetworkInterface] ConnectFailed: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection failed");
        }

        public void Event_ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            DebugService.Log("[PhotonNetworkInterface] ConnectRefused: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection refused");
        }

        public void Event_ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            DebugService.Log("[PhotonNetworkInterface] ConnectRequest: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            // will clear the session list in X seconds
            _sessionClearTimer = SESSION_CLEAR_TIMEOUT;
            onSessionListUpdated?.Invoke();
        }

        public void Event_SessionConnectFailed(UdpSession session, IProtocolToken token)
        {
            DebugService.Log("[PhotonNetworkInterface] SessionConnectFailed: " + session.Id + "  name: " + session.HostName);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, null);
        }

        public void Event_SessionCreated(UdpSession session)
        {
            DebugService.Log("[PhotonNetworkInterface] SessionCreated: " + session.Id + "  name: " + session.HostName);
            _connectedSessionInfo = new PhotonNetworkInterfaceSession(session);

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, true, null);
        }

        public void Event_SessionCreationFailed(UdpSession session)
        {
            DebugService.Log("[PhotonNetworkInterface] SessionCreationFailed: " + session.Id + "  name: " + session.HostName);

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, false, null);
        }

        public void Event_OnEvent(BoltCommunicationEvent evnt)
        {
            INetworkInterfaceConnection connection = _connections.Find((con) => con.Id == evnt.RaisedBy.ConnectionId);

            if (connection == null)
            {
                DebugService.Log("[PhotonNetworkInterface] Failed to find who raised the event: " + evnt.GetType() + " / " + evnt.RaisedBy);
                return;
            }

            DebugService.Log("[PhotonNetworkInterface] OnEvent: (length)" + evnt.BinaryData.Length);
            _messageReader?.Invoke(connection, evnt.BinaryData, evnt.BinaryData.Length);
        }
        #endregion

        #region Private Methods
        void CreateBoltListener()
        {
            if (_photonCallbackListener == null)
            {
                _photonCallbackListener = new GameObject("PhotonNetworkCallbackListener");
                var listenerComponent = _photonCallbackListener.AddComponent<PhotonNetworkCallbackListener>();
                listenerComponent.photonNetworkInterface = this;
                UnityEngine.Object.DontDestroyOnLoad(_photonCallbackListener);
            }
        }

        void DestroyListener()
        {
            if (_photonCallbackListener != null)
            {
                UnityEngine.Object.Destroy(_photonCallbackListener);
                _photonCallbackListener = null;
            }
        }

        void RegisterTokenClasses()
        {
            // class needed to set room properties
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        }

        void ConcludeOperationCallback(ref OperationResultCallback callback, bool success, string message)
        {
            callback?.Invoke(success, message);
            callback = null;
        }
        #endregion

        Action<INetworkInterfaceConnection, byte[], int> _messageReader;
        List<INetworkInterfaceConnection> _connections = new List<INetworkInterfaceConnection>();
        INetworkInterfaceSession _connectedSessionInfo;
        GameObject _photonCallbackListener;
        float _sessionClearTimer; // used to clear the session list after a timeout
        const float SESSION_CLEAR_TIMEOUT = 7;

        OperationResultCallback _operationCallbackLaunch;
        OperationResultCallback _operationCallbackShutdown;
        OperationResultCallback _operationCallbackSessionCreated;
        OperationResultCallback _operationCallbackSessionConnected;
        OperationResultCallback _operationCallbackDisconnectedFromSession;
    }
}