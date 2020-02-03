using System;
using System.Collections.Generic;

public class SimulationControllerServer : SimulationControllerMaster
{
    SessionServerInterface _session;

    SimulationSyncServerOperation _syncOp;

    List<INetworkInterfaceConnection> _clientsWaitingForSync = new List<INetworkInterfaceConnection>();

    public override void OnGameReady()
    {
        base.OnGameReady();

        _session = OnlineService.ServerInterface.sessionServerInterface;
        _session.RegisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
        _session.RegisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        if (_session != null)
        {
            _session.UnregisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);
            _session.UnregisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
            _session = null;
        }
    }

    void OnNetMessageInputSubmission(NetMessageInputSubmission netMessage, INetworkInterfaceConnection source)
    {
        // A client wants to submit a new message
        PlayerInfo sourcePlayer = PlayerRepertoireServer.Instance.GetPlayerInfo(source);

        if (ValidateInputSubmission(netMessage, sourcePlayer))
            QueueInput(netMessage.input, sourcePlayer, netMessage.submissionId);
    }

    void OnSimSyncRequest(NetMessageRequestSimSync netMessage, INetworkInterfaceConnection clientConnection)
    {
        // A client wants a complete simulatio sync
        DebugService.Log($"Client {clientConnection.Id} requested a simulation sync");

        if (!TryLaunchSyncForClient(clientConnection))
        {
            DebugService.Log($"Could not start sync right now. Will try later automatically.");
            _clientsWaitingForSync.AddUnique(clientConnection);
        }
    }

    bool ValidateInputSubmission(NetMessageInputSubmission submission, PlayerInfo playerInfo)
    {
        // This should eventually evolve into a full validation check (prevent cheating)
        if (!SimulationView.IsRunningOrReadyToRun)
            return false;

        if (playerInfo == null)
            return false;

        return true;
    }

    protected override void OnAboutToTickSimulation(ApprovedSimInput[] inputs)
    {
        base.OnAboutToTickSimulation(inputs);

        // Send tick to clients!
        NetMessageSimTick netMessage = new NetMessageSimTick()
        {
            tickId = SimulationView.TickId,
            inputs = inputs
        };

        _session.SendNetMessage(netMessage, PlayerRepertoireServer.Instance.PlayerConnections);
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (_clientsWaitingForSync.Count > 0)
        {
            for (int i = _clientsWaitingForSync.Count - 1; i >= 0; i--)
            {
                if (!_session.IsConnectionValid(_clientsWaitingForSync[i])
                    || TryLaunchSyncForClient(_clientsWaitingForSync[i]))
                {
                    _clientsWaitingForSync.RemoveAt(i);
                }
            }
        }
    }

    bool TryLaunchSyncForClient(INetworkInterfaceConnection clientConnection)
    {
        if (_syncOp == null || !_syncOp.IsRunning)
        {
            DebugService.Log($"Starting new sync...");
            _syncOp = new SimulationSyncServerOperation(_session, clientConnection);

            _syncOp.OnFailCallback = (op) =>
            {
                DebugService.Log($"Sync failed. {op.Message}");
            };

            _syncOp.OnSucceedCallback = (op) =>
            {
                DebugService.Log($"Sync complete. {op.Message}");
            };

            _syncOp.Execute();

            return true;
        }
        else if (_syncOp.CanAcceptNewClients)
        {
            DebugService.Log($"Adding a client to ongoing sync...");
            _syncOp.AddAdditionalClientConnection(clientConnection);

            return true;
        }
        else
        {
            // cannot sync right now...
            return false;
        }
    }
}
