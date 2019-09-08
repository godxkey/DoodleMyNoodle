using System;
using System.Collections.Generic;

public class SimulationControllerServer : SimulationController
{
    SessionServerInterface _session;
    Queue<ApprovedSimInput> _inputQueue = new Queue<ApprovedSimInput>();


    public bool allowSimToTick = false;

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

        PlayerInfo localPlayer = PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
        QueueInput(input, localPlayer, new InputSubmissionId(0));
    }

    void OnNetMessageInputSubmission(NetMessageInputSubmission submission, INetworkInterfaceConnection source)
    {
        // A client wants to submit a new message
        PlayerInfo sourcePlayer = PlayerRepertoireSystem.Instance.GetPlayerInfo(source);

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

    void QueueInput(SimInput input, PlayerInfo playerInfo, InputSubmissionId submissionId)
    {
        if (!allowSimToTick)
            return;

        if(input is SimPlayerInput playerInput)
        {
            if (playerInfo.SimPlayerId.IsValid)
            {
                // Set SimPlayerId in input
                // fbessette: Currently, since the 'SimPlayerId' is a field in the inputs, the clients pointlessly have that data sent to us.
                //            To optimize the package's size, we could refactor this a little and pull the SimPlayerId out of the base input data class
                playerInput.SimPlayerId = playerInfo.SimPlayerId; 
            }
            else
            {
                DebugService.Log($"[{nameof(SimulationControllerServer)}] We refused {playerInfo.PlayerName}'s input because he doesn't have a " +
                    $"valid SimPlayerId yet.");
                return; // player cannot submit inputs yet
            }
        }

        _inputQueue.Enqueue(new ApprovedSimInput()
        {
            input = input,
            playerInstigator = playerInfo.PlayerId,
            clientSubmissionId = submissionId
        });
    }

    void FixedUpdate()
    {
        if (!Game.started)
            return;

        if (SimulationView.CanBeTicked && allowSimToTick)
        {
            AssignSimPlayerIdsToPlayersMissingOne();
            TickSimulation();
        }
        else
        {
            _inputQueue.Clear();
        }
    }

    void TickSimulation()
    {
        ApprovedSimInput[] inputsForThisTick = _inputQueue.ToArray();
        _inputQueue.Clear();

        // Send tick to clients!
        NetMessageSimTick netMessage = new NetMessageSimTick()
        {
            tickId = SimulationView.TickId,
            inputs = inputsForThisTick
        };

        _session.SendNetMessage(netMessage, PlayerRepertoireServer.Instance.PlayerConnections);


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

        SimulationView.Tick(tickData);
    }

    void AssignSimPlayerIdsToPlayersMissingOne()
    {
        if (SimPlayerManager.Instance == null)
            return;

        // fbessette: For now, we assign SimPlayers randomly. First player arrived gets the first SimPlayer
        //            Eventually, we'll want returning players to get back their old SimPlayer to keep their character/gear/etc.

        foreach (PlayerInfo playerInfo in PlayerRepertoireSystem.Instance.Players)
        {
            if (PlayerIdHelpers.GetSimPlayerFromPlayer(playerInfo) == null)
            {
                ISimPlayerInfo unassignedSimPlayer = GetUnassignedSimPlayer();

                if (unassignedSimPlayer != null)
                {
                    PlayerRepertoireServer.Instance.AssignSimPlayerToPlayer(playerInfo.PlayerId, unassignedSimPlayer.SimPlayerId);
                }
                else
                {
                    SimPlayerInfo newSimPlayerInfo = new SimPlayerInfo();
                    newSimPlayerInfo.Name = playerInfo.PlayerName;

                    SubmitInput(new SimInputPlayerCreate() { SimPlayerInfo = newSimPlayerInfo }); // ask the simulation to create a new player
                }
            }
        }
    }

    ISimPlayerInfo GetUnassignedSimPlayer()
    {
        if (SimPlayerManager.Instance == null)
            return null;

        foreach (ISimPlayerInfo simPlayer in SimPlayerManager.Instance.Players)
        {
            if (PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayer) == null)
            {
                return simPlayer;
            }
        }

        return null;
    }
}
