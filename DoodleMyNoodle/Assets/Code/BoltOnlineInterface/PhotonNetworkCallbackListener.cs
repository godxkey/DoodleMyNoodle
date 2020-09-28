using System;
using Bolt;
using UdpKit;

namespace Internals.PhotonNetworkInterface
{
    public class PhotonNetworkCallbackListener : Bolt.GlobalEventListener
    {
        public PhotonNetworkInterface PhotonNetworkInterface;

        public override bool PersistBetweenStartupAndShutdown() => true;

        public override void BoltStartBegin() => PhotonNetworkInterface.Event_BoltStartBegin();
        public override void BoltStartDone() => PhotonNetworkInterface.Event_BoltStartDone();
        public override void BoltStartFailed(UdpConnectionDisconnectReason disconnectReason) => PhotonNetworkInterface.Event_BoltStartFailed(disconnectReason);
        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)=> PhotonNetworkInterface.Event_BoltShutdownBegin(registerDoneCallback, disconnectReason);

        public override void Connected(BoltConnection connection) => PhotonNetworkInterface.Event_Connected(connection);
        public override void Disconnected(BoltConnection connection) => PhotonNetworkInterface.Event_Disconnected(connection);

        public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token) => PhotonNetworkInterface.Event_ConnectAttempt(endpoint, token);
        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token) => PhotonNetworkInterface.Event_ConnectFailed(endpoint, token);
        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token) => PhotonNetworkInterface.Event_ConnectRefused(endpoint, token);
        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token) => PhotonNetworkInterface.Event_ConnectRequest(endpoint, token);

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) => PhotonNetworkInterface.Event_SessionListUpdated(sessionList);
        public override void SessionConnectFailed(UdpSession session, IProtocolToken token, UdpSessionError errorReason) => PhotonNetworkInterface.Event_SessionConnectFailed(session, token, errorReason);
        public override void SessionCreated(UdpSession session) => PhotonNetworkInterface.Event_SessionCreated(session);
        public override void SessionCreationFailed(UdpSession session, UdpSessionError errorReason) => PhotonNetworkInterface.Event_SessionCreationFailed(session, errorReason);
        public override void SessionConnected(UdpSession session, IProtocolToken token) => PhotonNetworkInterface.Event_SessionConnected(session, token);

        public override void StreamDataStarted(BoltConnection connection, UdpChannelName channel, ulong streamID) => PhotonNetworkInterface.Event_StreamDataStarted(connection, channel, streamID);
        public override void StreamDataProgress(BoltConnection connection, UdpChannelName channel, ulong streamID, float progress) => PhotonNetworkInterface.Event_StreamDataProgress(connection, channel, streamID, progress);
        public override void StreamDataAborted(BoltConnection connection, UdpChannelName channel, ulong streamID) => PhotonNetworkInterface.Event_StreamDataAborted(connection, channel, streamID);
        public override void StreamDataReceived(BoltConnection connection, UdpStreamData data) => PhotonNetworkInterface.Event_StreamDataReceived(connection, data);

        public override void OnEvent(BoltCommunicationEvent evnt) => PhotonNetworkInterface.Event_OnEvent(evnt);
    }
}