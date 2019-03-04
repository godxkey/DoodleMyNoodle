using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;
using UnityEngine;

public class PhotonNetworkCallbackListener : Bolt.GlobalEventListener
{
    public PhotonNetworkInterface photonNetworkInterface;

    public override void BoltStartBegin() => photonNetworkInterface.Event_BoltStartBegin();
    public override void BoltStartDone() => photonNetworkInterface.Event_BoltStartDone();
    public override void BoltStartFailed() => photonNetworkInterface.Event_BoltStartFailed();
    public override void BoltShutdownBegin(AddCallback registerDoneCallback) => photonNetworkInterface.Event_BoltShutdownBegin(registerDoneCallback);
    public override void Connected(BoltConnection connection) => photonNetworkInterface.Event_Connected(connection);
    public override void Disconnected(BoltConnection connection) => photonNetworkInterface.Event_Disconnected(connection);
    public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token) => photonNetworkInterface.Event_ConnectAttempt(endpoint, token);
    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token) => photonNetworkInterface.Event_ConnectFailed(endpoint, token);
    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token) => photonNetworkInterface.Event_ConnectRefused(endpoint, token);
    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token) => photonNetworkInterface.Event_ConnectRequest(endpoint, token);
    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) => photonNetworkInterface.Event_SessionListUpdated(sessionList);
    public override void SessionConnectFailed(UdpSession session, IProtocolToken token) => photonNetworkInterface.Event_SessionConnectFailed(session, token);
    public override void SessionCreated(UdpSession session) => photonNetworkInterface.Event_SessionCreated(session);
    public override void SessionCreationFailed(UdpSession session) => photonNetworkInterface.Event_SessionCreationFailed(session);
    public override void OnEvent(BoltCommunicationEvent evnt) => photonNetworkInterface.Event_OnEvent(evnt);
}
