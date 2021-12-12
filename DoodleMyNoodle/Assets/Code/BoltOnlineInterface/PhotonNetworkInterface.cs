using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

namespace Internals.PhotonNetworkInterface
{
    internal class PhotonNetworkInterfaceConnection : INetworkInterfaceConnection
    {
        public BoltConnection BoltConnection { get; private set; }

        public PhotonNetworkInterfaceConnection(BoltConnection boltConnection)
        {
            BoltConnection = boltConnection;
        }

        public uint Id => BoltConnection.ConnectionId;

        public void SetStreamBandwidth(int bytesPerSecond)
        {
            BoltConnection.SetStreamBandwidth(bytesPerSecond);
        }

        public void StreamBytes(IStreamChannel channel, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Log.Info(PhotonNetworkInterface.LogChannel, $"[PhotonNetworkInterface] Outgoing stream started ({data.Length} bytes).");
            BoltConnection.StreamBytes(((StreamChannel)channel).UdpChannelName, data);
        }
    }

    public class StreamChannel : IStreamChannel
    {
        public UdpChannelName UdpChannelName;

        public StreamChannel(UdpChannelName udpChannelName, string name, bool isReliable, int priority, StreamChannelType type)
        {
            UdpChannelName = udpChannelName;
            Name = name;
            IsReliable = isReliable;
            Priority = priority;
            Type = type;
        }

        public string Name { get; private set; }

        public bool IsReliable { get; private set; }

        public int Priority { get; private set; }

        public StreamChannelType Type { get; private set; }
    }

    public class PhotonNetworkInterface : NetworkInterface
    {
        internal static LogChannel LogChannel = Log.CreateChannel("PhotonNetworkInterface", false);

        internal static PhotonNetworkInterface Instance { get; private set; }

        public override event Action OnDisconnectedFromSession;
        public override event Action OnShutdownBegin;
        public override event Action<INetworkInterfaceConnection> OnDisconnect;
        public override event Action<INetworkInterfaceConnection> OnConnect;
        public override event Action OnSessionListUpdated;
        public override event Action<byte[], IStreamChannel, INetworkInterfaceConnection> StreamDataReceived;
        public override event StreamDataStartedDelegate StreamDataStarted;
        public override event StreamDataProgressDelegate StreamDataProgress;
        public override event StreamDataAbortedDelegate StreamDataAborted;

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
            Instance = this;
        }

        public override void LaunchClient(OperationResultDelegate onComplete)
        {
            if (State != NetworkState.Stopped)
                throw new Exception("Cannot launch client when state is not 'Stopped'.");

            _operationCallbackLaunch = onComplete;
            State = NetworkState.Launching;
            Log.Info(LogChannel, "[PhotonNetworkInterface] LaunchClient...");
            BoltLauncher.StartClient(GameBoltConfig.GetConfig());
        }
        public override void LaunchServer(OperationResultDelegate onComplete)
        {
            if (State != NetworkState.Stopped)
                throw new Exception("Cannot launch server when state is not 'Stopped'.");

            _operationCallbackLaunch = onComplete;
            State = NetworkState.Launching;
            Log.Info(LogChannel, "[PhotonNetworkInterface] LaunchServer...");
            BoltLauncher.StartServer(GameBoltConfig.GetConfig());
        }

        public override void Shutdown(OperationResultDelegate onComplete)
        {
            _operationCallbackShutdown = onComplete;
            State = NetworkState.ShuttingDown;
            BoltNetwork.Shutdown();
        }

        public override void CreateSession(string sessionName, OperationResultDelegate onComplete)
        {
            if (State != NetworkState.Running)
            {
                Log.Info(LogChannel, "[PhotonNetworkInterface] Cannot create session if not in running state");
                return;
            }

            if (!BoltNetwork.IsServer)
            {
                Log.Info(LogChannel, "[PhotonNetworkInterface] Cannot create session if not of type server");
                return;
            }

            _operationCallbackSessionCreated = onComplete;

            // Create some room custom properties
            PhotonRoomProperties roomProperties = new PhotonRoomProperties();

            roomProperties.IsOpen = true;
            roomProperties.IsVisible = true;

            // Create the Photon Room
            BoltMatchmaking.CreateSession(sessionName, roomProperties);

            Log.Info(LogChannel, "[PhotonNetworkInterface] Session creation begun...");
        }

        public override void ConnectToSession(INetworkInterfaceSession session, OperationResultDelegate onComplete)
        {
            _operationCallbackSessionConnected = onComplete;

            UdpSession udpSession = ((PhotonNetworkInterfaceSession)session).UdpSession;

            BoltMatchmaking.JoinSession(udpSession);

            _connectedSessionInfo = session;
        }

        public override void DisconnectFromSession(OperationResultDelegate onComplete)
        {
            //_operationCallbackDisconnectedFromSession = onComplete;
            _connectedSessionInfo = null;
            Shutdown(onComplete); // with bolt, we have no way of returning to 'lobby' state
        }

        public override void Update()
        {
            if (State == NetworkState.ShuttingDown && !BoltNetwork.IsRunning)
            {
                // This timer inserts a delay between Bolt's alleged 'Shutdown complete' and our state 'Stopped'
                // This an ugly fix to a bug where requesting a StartClient or StartServer too soon after the shutdown breaks Bolt's state
                if (_shutdownCompleteTimer > 0)
                {
                    _shutdownCompleteTimer -= Time.unscaledDeltaTime;
                }
                else
                {
                    Log.Info(LogChannel, "[PhotonNetworkInterface] BoltShutdownComplete");
                    State = NetworkState.Stopped;

                    if (_connections.Count != 0)
                    {
                        _connections.Clear();
                        Log.Info(LogChannel, "[PhotonNetworkInterface] Shutdown weirdness: m_connections.Count > 0");
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
        }

        public override void SendMessage(INetworkInterfaceConnection connection, byte[] data, bool reliableAndOrdered)
        {
            if (connection == null)
            {
                Log.Error("[PhotonNetworkInterface] Cannot send message to null connection");
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
            Instance = null;
            DestroyListener();
        }

        public override IStreamChannel GetStreamChannel(StreamChannelType channel)
        {
            return _streamChannels[channel];
        }

        #region Event Handling
        public void Event_BoltStartBegin()
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] BoltStartBegin");

            // class needed to set room properties
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();

            CreateStreamChannels();
        }

        public void Event_BoltStartDone()
        {
            State = NetworkState.Running;
            Log.Info(LogChannel, "[PhotonNetworkInterface] BoltStartDone");

            _operationCallbackLaunch?.Invoke(true, null);
        }

        public void Event_BoltStartFailed(UdpConnectionDisconnectReason disconnectReason)
        {
            State = NetworkState.Stopped;
            Log.Info(LogChannel, "[PhotonNetworkInterface] BoltStartFailed: " + disconnectReason);

            _operationCallbackLaunch?.Invoke(false, "Bolt failed to start: " + disconnectReason);
        }

        public void Event_BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            _shutdownCompleteTimer = BOLT_SHUTDOWNCOMPLETE_DELAY;
            State = NetworkState.ShuttingDown;
            Log.Info(LogChannel, "[PhotonNetworkInterface] BoltShutdownBegin: " + disconnectReason);

            OnShutdownBegin?.Invoke();
        }

        public void Event_Connected(BoltConnection connection)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] Connected: " + connection.ToString());
            _connections.Add(new PhotonNetworkInterfaceConnection(connection));


            // NOTE: this event will get called for each new connection
            //     client: called once when we join the session
            //     server: called multiple times
            ConcludeOperationCallback(ref _operationCallbackSessionConnected, true, null);

            OnConnect?.Invoke(_connections.Last());
        }

        public void Event_Disconnected(BoltConnection connection)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] Disconnected: " + connection.ToString());

            INetworkInterfaceConnection connectionInterface = FindInterfaceConnection(connection);

            if (connectionInterface != null)
            {
                _connections.Remove(connectionInterface);
                OnDisconnect?.Invoke(connectionInterface);
            }

            if (IsClient && _connections.Count == 0 && _connectedSessionInfo != null)
            {
                ConcludeOperationCallback(ref _operationCallbackDisconnectedFromSession, success: true, message: null);

                OnDisconnectedFromSession?.Invoke();
                _connectedSessionInfo = null;
            }
        }

        public void Event_ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] ConnectAttempt: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] ConnectFailed: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection failed");
        }

        public void Event_ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] ConnectRefused: " + endpoint.Address + "  port: " + endpoint.Port);

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, "Connection refused");
        }

        public void Event_ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            Log.Info(LogChannel, "[PhotonNetworkInterface] ConnectRequest: " + endpoint.Address + "  port: " + endpoint.Port);
        }

        public void Event_SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            _sessions.Clear();

            foreach (KeyValuePair<Guid, UdpSession> session in sessionList)
            {
                _sessions.Add(new PhotonNetworkInterfaceSession(session.Value));
            }

            OnSessionListUpdated?.Invoke();
        }

        public void Event_SessionConnectFailed(UdpSession session, IProtocolToken token, UdpSessionError errorReason)
        {
            Log.Info(LogChannel, $"[PhotonNetworkInterface] SessionConnectFailed: session:{session.Id} name:{session.HostName} reason:{errorReason}");

            ConcludeOperationCallback(ref _operationCallbackSessionConnected, false, errorReason.ToString());
        }

        public void Event_SessionCreatedOrUpdated(UdpSession session)
        {
            Log.Info(LogChannel, $"[PhotonNetworkInterface] SessionCreatedOrUpdated: session:{session.Id} name:{session.HostName}");
            _connectedSessionInfo = new PhotonNetworkInterfaceSession(session);

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, true, null);
        }

        public void Event_SessionCreationFailed(UdpSession session, UdpSessionError errorReason)
        {
            Log.Info(LogChannel, $"[PhotonNetworkInterface] SessionCreationFailed: session:{session.Id} name:{session.HostName} reason:{errorReason}");

            ConcludeOperationCallback(ref _operationCallbackSessionCreated, false, null);
        }

        public void Event_SessionConnected(UdpSession session, IProtocolToken token)
        {
            // nothing to do at the moment
        }

        public void Event_OnEvent(BoltCommunicationEvent evnt)
        {
            INetworkInterfaceConnection connection = _connections.Find((con) => con.Id == evnt.RaisedBy.ConnectionId);

            if (connection == null)
            {
                Log.Info(LogChannel, "[PhotonNetworkInterface] Failed to find who raised the event: " + evnt.GetType() + " / " + evnt.RaisedBy);
                return;
            }

            _messageReader?.Invoke(connection, evnt.BinaryData);
        }


        public void Event_StreamDataStarted(BoltConnection connection, UdpChannelName channel, ulong streamID)
        {
            INetworkInterfaceConnection interfaceConnection = FindInterfaceConnection(connection);

            if (interfaceConnection == null)
            {
                Log.Error($"[PhotonNetworkInterface] StreamDataStarted from unknown connection: {connection}.");
                return;
            }

            IStreamChannel streamChannel = FindStreamChannel(channel);

            if (streamChannel == null)
            {
                Log.Error($"[PhotonNetworkInterface] StreamDataStarted from an unknown channel '{channel}'.");
                return;
            }

            Log.Info(LogChannel, $"[PhotonNetworkInterface] Incoming stream data started: (streamID){streamID}");

            StreamDataStarted?.Invoke(interfaceConnection, streamChannel, streamID);
        }

        public void Event_StreamDataProgress(BoltConnection connection, UdpChannelName channel, ulong streamID, float progress)
        {
            INetworkInterfaceConnection interfaceConnection = FindInterfaceConnection(connection);

            if (interfaceConnection == null)
            {
                Log.Error($"[PhotonNetworkInterface] StreamDataProgress from unknown connection: {connection}.");
                return;
            }

            IStreamChannel streamChannel = FindStreamChannel(channel);

            if (streamChannel == null)
            {
                Log.Error($"[PhotonNetworkInterface] StreamDataProgress from an unknown channel '{channel}'.");
                return;
            }

            StreamDataProgress?.Invoke(interfaceConnection, streamChannel, streamID, progress);
        }

        public void Event_StreamDataAborted(BoltConnection connection, UdpChannelName channel, ulong streamID)
        {
            INetworkInterfaceConnection interfaceConnection = FindInterfaceConnection(connection);

            if (interfaceConnection == null)
            {
                Log.Info($"[PhotonNetworkInterface] StreamDataAborted from unknown connection: {connection}.");
            }

            IStreamChannel streamChannel = FindStreamChannel(channel);

            if (streamChannel == null)
            {
                Log.Error($"[PhotonNetworkInterface] StreamDataAborted from an unknown channel '{channel}'.");
                return;
            }

            Log.Info(LogChannel, $"[PhotonNetworkInterface] Incoming stream aborted: (streamID){streamID}");

            StreamDataAborted?.Invoke(interfaceConnection, streamChannel, streamID);
        }

        public void Event_StreamDataReceived(BoltConnection connection, UdpStreamData streamData)
        {
            INetworkInterfaceConnection interfaceConnection = FindInterfaceConnection(connection);

            if (interfaceConnection == null)
            {
                Log.Error($"[PhotonNetworkInterface] Received stream data from an unknown connection: {connection}.");
                return;
            }

            IStreamChannel streamChannel = FindStreamChannel(streamData.Channel);

            if (streamChannel == null)
            {
                Log.Error($"[PhotonNetworkInterface] Received stream data from an unknown channel '{streamData.Channel}'.");
                return;
            }

            Log.Info(LogChannel, "[PhotonNetworkInterface] Incoming stream data received: (length)" + streamData.Data.Length);

            StreamDataReceived?.Invoke(streamData.Data, streamChannel, interfaceConnection);

        }
        #endregion

        #region Private Methods
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

                StreamChannel channel = new StreamChannel(photonChannelName, streamChannelNames[i], reliable, priority, type);

                _streamChannels.Add(type, channel);
            }
        }

        INetworkInterfaceConnection FindInterfaceConnection(BoltConnection boltConnection)
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                if (_connections[i].Id == boltConnection.ConnectionId)
                {
                    return _connections[i];
                }
            }
            return null;
        }

        IStreamChannel FindStreamChannel(UdpChannelName channelName)
        {
            IEqualityComparer<UdpChannelName> equalityComparer = UdpChannelName.EqualityComparer.Instance;
            foreach (var item in _streamChannels)
            {
                if (equalityComparer.Equals(((StreamChannel)item.Value).UdpChannelName, channelName))
                {
                    return item.Value;
                }
            }
            return null;
        }

        void CreateBoltListener()
        {
            if (_photonCallbackListener == null)
            {
                _photonCallbackListener = new GameObject("PhotonNetworkCallbackListener");
                var listenerComponent = _photonCallbackListener.AddComponent<PhotonNetworkCallbackListener>();
                listenerComponent.PhotonNetworkInterface = this;
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

        OperationResultDelegate _operationCallbackLaunch;
        OperationResultDelegate _operationCallbackShutdown;
        OperationResultDelegate _operationCallbackSessionCreated;
        OperationResultDelegate _operationCallbackSessionConnected;
        OperationResultDelegate _operationCallbackDisconnectedFromSession;

        // This timer inserts a delay between Bolt's alleged 'Shutdown complete' and our state 'Stopped'
        // This an ugly fix to a bug where requesting a StartClient or StartServer too soon after the shutdown breaks Bolt's state
        private const float BOLT_SHUTDOWNCOMPLETE_DELAY = 1f;
        private float _shutdownCompleteTimer;
    }
}