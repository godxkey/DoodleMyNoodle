using System;
using System.Collections.Generic;

public class SimulationControllerServer : SimulationControllerMaster
{
    SessionServerInterface _session;

    public override void OnGameReady()
    {
        base.OnGameReady();

        _session = OnlineService.serverInterface.sessionServerInterface;
        _session.RegisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        _session?.UnregisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
        _session = null;
    }

    void OnNetMessageInputSubmission(NetMessageInputSubmission submission, INetworkInterfaceConnection source)
    {
        // A client wants to submit a new message
        PlayerInfo sourcePlayer = PlayerRepertoireServer.Instance.GetPlayerInfo(source);

        if (ValidateInputSubmission(submission, sourcePlayer))
            QueueInput(submission.input, sourcePlayer, submission.submissionId);
    }

    bool ValidateInputSubmission(NetMessageInputSubmission submission, PlayerInfo playerInfo)
    {
        // This should eventually evolve into a full validation check (prevent cheating)
        if (!SimulationView.IsInitialized)
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
}
