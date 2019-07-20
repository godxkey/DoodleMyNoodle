using System;
using System.Collections.Generic;

public class SimulationControllerServer : SimulationController
{
    SessionServerInterface _session;
    Queue<ApprovedSimInput> inputQueue = new Queue<ApprovedSimInput>();

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

    public override void SubmitInput(SimInput input)
    {
        if (input == null)
        {
            DebugService.LogError("Trying to submit a null input");
            return;
        }

        PlayerInfo localPlayer = PlayerRepertoireSystem.instance.GetLocalPlayerInfo();
        QueueInput(input, localPlayer, new InputSubmissionId(0));
    }

    void OnNetMessageInputSubmission(NetMessageInputSubmission submission, INetworkInterfaceConnection source)
    {
        // A client wants to submit a new message
        PlayerInfo sourcePlayer = PlayerRepertoireSystem.instance.GetPlayerInfo(source);

        if (ValidateInputSubmission(submission, sourcePlayer))
            QueueInput(submission.input, sourcePlayer, submission.submissionId);
    }

    bool ValidateInputSubmission(NetMessageInputSubmission submission, PlayerInfo playerInfo)
    {
        // This should eventually evolve into a full validation check (prevent cheating)
        if (!Simulation.isInitialized)
            return false;

        if (playerInfo == null)
            return false;

        return true;
    }

    void QueueInput(SimInput input, PlayerInfo playerInfo, InputSubmissionId submissionId)
    {
        inputQueue.Enqueue(new ApprovedSimInput()
        {
            input = input,
            playerInstigator = playerInfo.playerId,
            clientSubmissionId = submissionId
        });
    }

    void FixedUpdate()
    {
        if (!Game.started)
            return;

        if (Simulation.canBeTicked)
        {
            ApprovedSimInput[] inputsForThisTick = inputQueue.ToArray();
            inputQueue.Clear();

            // Send tick to clients!
            NetMessageSimTick netMessage = new NetMessageSimTick()
            {
                tickId = Simulation.tickId,
                inputs = inputsForThisTick
            };

            _session.SendNetMessage(netMessage, PlayerRepertoireServer.instance.playerConnections);


            // Tick the simulation locally
            SimInput[] simInputs = new SimInput[inputsForThisTick.Length];
            for (int i = 0; i < inputsForThisTick.Length; i++)
            {
                simInputs[i] = inputsForThisTick[i].input;
            }

            SimTickData tickData = new SimTickData()
            {
                inputs = simInputs
            };

            Simulation.Tick(tickData);
        }
        else
        {
            inputQueue.Clear();
        }

    }
}
