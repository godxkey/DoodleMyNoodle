using Bolt;
using Bolt.photon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UdpKit;
using UnityEngine;

namespace Internals.PhotonNetwokInterface
{
    public class StreamChannel : IStreamChannel
    {
        public UdpChannelName UdpChannelName;

        public StreamChannel(UdpChannelName udpChannelName, string name, bool isReliable, int priority)
        {
            UdpChannelName = udpChannelName;
            Name = name;
            IsReliable = isReliable;
            Priority = priority;
        }

        public string Name { get; private set; }

        public bool IsReliable { get; private set; }

        public int Priority { get; private set; }
    }

    public class PhotonNetworkInterface : NetworkInterface
    {
        const bool log = true;
        const bool intenseLog = false;

        public override event Action OnDisconnectedFromSession;
        public override event Action OnShutdownBegin;
        public override event Action<INetworkInterfaceConnection> OnDisconnect;
        public override event Action<INetworkInterfaceConnection> OnConnect;
        public override event Action OnSessionListUpdated;

        public override bool IsServer => BoltNetwork.IsServer;
        public override ReadOnlyList<INetworkInterfaceConnection> Connections => _connections.AsReadOnlyNoAlloc();
        public override ReadOnlyList<INetworkInterfaceSession> Sessions => _sessions.AsReadOnlyNoAlloc();
        public override INetworkInterfaceSession ConnectedSessionInfo => _connectedSessionInfo;
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
            CreateBoltListener();
        }

        public override void LaunchClient(OperationResultDelegate onComplete)
        {
            _operationCallbackLaunch = onComplete;
            State = NetworkState.Launching;
            BoltLauncher.StartClient();
        }
        public override void LaunchServer(OperationResultDelegate onComplete)
        {
            _operationCallbackLaunch = onComplete;
            State = NetworkState.Launching;
            BoltLauncher.StartServer();
        }

        public override void Shutdown(OperationResultDelegate onComplete)
        {
            _operationCallbackShutdown = onComplete;
            State = NetworkState.ShuttingDown;
            BoltLauncher.Shutdown();
        }

        public override void CreateSession(string sessionName, OperationResultDelegate onComplete)
        {
            if (State != NetworkState.Running)
            {
                if (log)
                    DebugService.Log("[PhotonNetworkInterface] Cannot create session if not in running state");
                return;
            }

            if (!BoltNetwork.IsServer)
            {
                if (log)
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

            if (log)
                DebugService.Log("[PhotonNetworkInterface] Session creation begun...");
        }

        public override void ConnectToSession(INetworkInterfaceSession session, OperationResultDelegate onComplete)
        {
            _operationCallbackSessionConnected = onComplete;

            UdpSession udpSession = ((PhotonNetworkInterfaceSession)session).UdpSession;

            BoltNetwork.Connect(udpSession);

            _connectedSessionInfo = session;
        }

        public override void DisconnectFromSession(OperationResultDelegate onComplete)
        {
            _connectedSessionInfo = null;
            Shutdown(onComplete); // with bolt, we have no way of returning to 'lobby' state
        }

        public override void Update()
        {
            if (!IsServer)
            {
                if (_sessionClearTimer < 0 && BoltNetwork.IsRunning)
                {
                    // clear the session list. 
                    // This is needed because we apparently don't get session updates if there are no sessions
                    BoltNetwork.UpdateSessionList(new Map<Guid, UdpSession>());
                }

                _sessionClearTimer -= Time.deltaTime;
            }

            if (State == NetworkState.ShuttingDown && !BoltNetwork.IsRunning)
            {
                State = NetworkState.Stopped;

                if (_connections.Count != 0)
                {
                    _connections.Clear();
                    if (log)
                        DebugService.Log("[PhotonNetworkInterface] Shut down complete weirdness: m_connections.Count > 0");
                }

                if (_connectedSessionInfo != null)
                {
                    _connectedSessionInfo = null;
                    OnDisconnectedFromSession?.Invoke();
                }

                ConcludeOperationCallback(ref _operationCallbackDisconnectedFromSession, true, null);
                ConcludeOperationCallback(ref _operationCallbackShutdown, true, null);
            }
        }

        public override void SendMessage(INetworkInterfaceConnection connection, byte[] data, bool reliableAndOrdered)
        {
            if (connection == null)
            {
                DebugService.LogError("[PhotonNetworkInterface] Cannot send message to null connection");
                return;
            }

            BoltConnection boltConnection = ((PhotonNetworkInterfaceConnection)connection).BoltConnection;
            BoltCommunicationEvent evt = BoltCommunicationEvent.Create(boltConnection,
                reliableAndOrdered ? ReliabilityModes.ReliableOrdered : ReliabilityModes.Unreliable);

            if (evt != null) // evt might be null if the connection is not valid anymore
            {
                evt.BinaryData = data;
                evt.Send();
            }
        }

        public override void SetMessageReader(Action<INetworkInterfaceConnection, byte[]> messageReader)
        {
            _messageReader = messageReader;
        }

        public override void Dispose()
        {
            DestroyListener();
        }
        
        public override IStreamChannel GetStreamChannel(StreamChannelType channel)
        {
            return _streamChannels[channel];
        }

        #region Event Handling
        public void Event_BoltStartBegin()
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] BoltStartBegin");

            // class needed to set room properties
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();

            CreateStreamChannels();
        }

        private void CreateStreamChannels()
        {
            _streamChannels.Clear();

            string[] streamChannelNames = Enum.GetNames(typeof(StreamChannelType));
            int[] streamChannelValues = Enum.GetValues(typeof(StreamChannelType)) as int[];

            for (int i = 0; i < streamChannelValues.Length; i++)
            {
                StreamChannelType type = (StreamChannelType)streamChannelValues[i];

                GetStreamChannelSettings(type, out bool reliable, out int priority);

                UdpChannelName photonChannelName = BoltNetwork.CreateStreamChannel(streamChannelNames[i], reliable ? UdpChannelMode.Reliable : UdpChannelMode.Unreliable, priority);

                StreamChannel channel = new StreamChannel(photonChannelName, streamChannelNames[i], reliable, priority);

                _streamChannels.Add(type, channel);
            }
        }

        public void Event_BoltStartDone()
        {
            State = NetworkState.Running;
            if (log)
                DebugService.Log("[PhotonNetworkInterface] BoltStartDone");

            _operationCallbackLaunch?.Invoke(true, null);
        }

        public void Event_BoltStartFailed()
        {
            State = NetworkState.Stopped;
            if (log)
                DebugService.Log("[PhotonNetworkInterface] BoltStartFailed");

            _operationCallbackLaunch?.Invoke(false, "Bolt failed to start");
        }

        public void Event_BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            State = NetworkState.ShuttingDown;
            if (log)
                DebugService.Log("[PhotonNetworkInterface] BoltShutdownBegin");

            OnShutdownBegin?.Invoke();
        }

        public void Event_Connected(BoltConnection connection)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] Connected: " + connection.ToString());
            _connections.Add(new PhotonNetworkInterfaceConnection(connection));

            connection.SetStreamBandwidth(1024 * 2000);


            // NOTE: this event will get called for each new connection
            //     client: called once when we join the session
            //     server: called multiple times
            ConcludeOperationCallback(ref _operationCallbackSessionConnected, true, null);

            OnConnect?.Invoke(_connections.Last());
        }

        public void Event_Disconnected(BoltConnection connection)
        {
            if (log)
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

            OnDisconnect?.Invoke(connectionInterface);

            if (IsClient && _connections.Count == 0 && _connectedSessionInfo != null)
            {
                ConcludeOperationCallback(ref _operationCallbackDisconnectedFromSession, true, null);

                OnDisconnectedFromSession?.Invoke();
                _connectedSessionInfo = null;
            }
        }

        public void Event_ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] ConnectAttempt: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] ConnectFailed: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection failed");
        }

        public void Event_ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] ConnectRefused: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection refused");
        }

        public void Event_ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] ConnectRequest: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            // will clear the session list in X seconds
            _sessionClearTimer = SESSION_CLEAR_TIMEOUT;

            _sessions.Clear();
            foreach (KeyValuePair<Guid, UdpSession> session in BoltNetwork.SessionList)
            {
                _sessions.Add(new PhotonNetworkInterfaceSession(session.Value));
            }

            OnSessionListUpdated?.Invoke();
        }

        public void Event_SessionConnectFailed(UdpSession session, IProtocolToken token)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] SessionConnectFailed: " + session.Id + "  name: " + session.HostName);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, null);
        }

        public void Event_SessionCreated(UdpSession session)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] SessionCreated: " + session.Id + "  name: " + session.HostName);
            _connectedSessionInfo = new PhotonNetworkInterfaceSession(session);

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, true, null);
        }

        public void Event_SessionCreationFailed(UdpSession session)
        {
            if (log)
                DebugService.Log("[PhotonNetworkInterface] SessionCreationFailed: " + session.Id + "  name: " + session.HostName);

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, false, null);
        }

        public void Event_OnEvent(BoltCommunicationEvent evnt)
        {
            INetworkInterfaceConnection connection = _connections.Find((con) => con.Id == evnt.RaisedBy.ConnectionId);

            if (connection == null)
            {
                if (log)
                    DebugService.Log("[PhotonNetworkInterface] Failed to find who raised the event: " + evnt.GetType() + " / " + evnt.RaisedBy);
                return;
            }

            if (intenseLog)
                DebugService.Log("[PhotonNetworkInterface] OnEvent: (length)" + evnt.BinaryData.Length);
            _messageReader?.Invoke(connection, evnt.BinaryData);
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

        void ConcludeOperationCallback(ref OperationResultDelegate callback, bool success, string message)
        {
            callback?.Invoke(success, message);
            callback = null;
        }
        #endregion

        Action<INetworkInterfaceConnection, byte[]> _messageReader;
        List<INetworkInterfaceConnection> _connections = new List<INetworkInterfaceConnection>();
        List<INetworkInterfaceSession> _sessions = new List<INetworkInterfaceSession>();
        Dictionary<StreamChannelType, IStreamChannel> _streamChannels = new Dictionary<StreamChannelType, IStreamChannel>();
        INetworkInterfaceSession _connectedSessionInfo;
        GameObject _photonCallbackListener;
        float _sessionClearTimer; // used to clear the session list after a timeout
        const float SESSION_CLEAR_TIMEOUT = 7;

        OperationResultDelegate _operationCallbackLaunch;
        OperationResultDelegate _operationCallbackShutdown;
        OperationResultDelegate _operationCallbackSessionCreated;
        OperationResultDelegate _operationCallbackSessionConnected;
        OperationResultDelegate _operationCallbackDisconnectedFromSession;
    }
}