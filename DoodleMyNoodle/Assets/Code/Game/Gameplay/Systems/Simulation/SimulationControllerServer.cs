using CCC.Operations;
using SimulationControl;
using System;
using System.Collections.Generic;

public class SimulationControllerServer : SimulationControllerMaster
{
    //[ConfigVar("sim.pause_while_join", "true", description: "Should the simulation be paused while players are joining the game?")]
    //static ConfigVar s_pauseSimulationWhilePlayersAreJoining;

    //SessionServerInterface _session;

    //List<CoroutineOperation> _ongoingOperations = new List<CoroutineOperation>();

    //public override void OnGameReady()
    //{
    //    base.OnGameReady();

    //    //_session = OnlineService.ServerInterface.sessionServerInterface;
    //    //_session.RegisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
    //    //_session.RegisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);
    //}

    //public override void OnSafeDestroy()
    //{
    //    base.OnSafeDestroy();


    //    //if (_session != null)
    //    //{
    //    //    _session.UnregisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);
    //    //    _session.UnregisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
    //    //    _session = null;
    //    //}

    //    //_ongoingOperations.ForEach((x) => x.TerminateWithFailure());
    //}

    //public override void OnGameUpdate()
    //{
    //    base.OnGameUpdate();

    //    if (_ongoingOperations.Count > 0)
    //    {
    //        _ongoingOperations.RemoveAll((x) => !x.IsRunning);
    //        if (_ongoingOperations.Count == 0 && s_pauseSimulationWhilePlayersAreJoining.BoolValue)
    //        {
    //            UnpauseSimulation(key: "PlayerJoining");
    //        }
    //    }
    //}

    //void OnNetMessageInputSubmission(NetMessageInputSubmission netMessage, INetworkInterfaceConnection source)
    //{
    //    // A client wants to submit a new message
    //    PlayerInfo sourcePlayer = PlayerRepertoireServer.Instance.GetPlayerInfo(source);

    //    if (ValidateInputSubmission(netMessage, sourcePlayer))
    //        QueueInput(netMessage.input, sourcePlayer, netMessage.submissionId);
    //}

    //void OnSimSyncRequest(NetMessageRequestSimSync netMessage, INetworkInterfaceConnection clientConnection)
    //{
    //    // A client wants a complete simulatio sync
    //    DebugService.Log($"Client {clientConnection.Id} requested a simulation sync");

    //    LaunchSyncForClient(clientConnection);
    //}

    //bool ValidateInputSubmission(NetMessageInputSubmission submission, PlayerInfo playerInfo)
    //{
    //    // This should eventually evolve into a full validation check (prevent cheating)
    //    if (!SimulationView.IsRunningOrReadyToRun)
    //        return false;

    //    if (playerInfo == null)
    //        return false;

    //    if (IsSimulationPaused)
    //        return false;

    //    return true;
    //}

    //protected override void OnAboutToTickSimulation(ApprovedSimInput[] inputs)
    //{
    //    base.OnAboutToTickSimulation(inputs);

    //    // Send tick to clients!
    //    NetMessageSimTick netMessage = new NetMessageSimTick()
    //    {
    //        tickId = SimulationView.TickId,
    //        Inputs = inputs
    //    };

    //    _session.SendNetMessage(netMessage, PlayerRepertoireServer.Instance.PlayerConnections);
    //}

    //SimulationSyncFromTransferServerOperation LaunchSyncForClient(INetworkInterfaceConnection clientConnection)
    //{
    //    DebugService.Log($"Starting new sync...");

    //    var newOp = new SimulationSyncFromTransferServerOperation(_session, clientConnection, SimulationWorld);

    //    newOp.OnFailCallback = (op) =>
    //    {
    //        DebugService.Log($"Sync failed. {op.Message}");
    //    };

    //    newOp.OnSucceedCallback = (op) =>
    //    {
    //        DebugService.Log($"Sync complete. {op.Message}");
    //    };

    //    newOp.Execute();

    //    _ongoingOperations.Add(newOp);

    //    if (s_pauseSimulationWhilePlayersAreJoining.BoolValue)
    //    {
    //        PauseSimulation(key: "PlayerJoining");
    //    }

    //    return newOp;
    //}
}
