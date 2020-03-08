using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;

public class SimulationControllerMaster : SimulationController
{
    [ConfigVar("sim.tick_rate", "1", description: "The number of ticks executed per fixed update")]
    static ConfigVar s_tickRate;

    private float _tickRateCounter;

    Queue<ApprovedSimInput> _inputQueue = new Queue<ApprovedSimInput>();

    // called by local player
    public override void SubmitInput(SimInput input)
    {
        if (input == null)
        {
            DebugService.LogError("Trying to submit a null input");
            return;
        }

        UnityEngine.Debug.Log("Input we submit :" + input);
        PlayerInfo localPlayer = PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
        QueueInput(input, localPlayer, new InputSubmissionId(0));
    }

    protected void QueueInput(SimInput input, PlayerInfo playerInfo, InputSubmissionId submissionId)
    {
        if (!CanTickSimulation) // we don't accept any input while sim is paused
            return;

        if (input is SimPlayerInput playerInput)
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
                DebugService.Log($"[{nameof(SimulationControllerMaster)}] We refused {playerInfo.PlayerName}'s input because he doesn't have a " +
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
        if (!Game.Started)
            return;

        SimulationView.UpdateSceneLoads();
        
        _tickRateCounter += s_tickRate.FloatValue;

        while (_tickRateCounter > 0)
        {
            if (CanTickSimulation)
            {

                AssignSimPlayerIdsToPlayersMissingOne();

                TickSimulation();
            }
            else
            {
                _inputQueue.Clear();
            }

            _tickRateCounter--;
        }
    }

    void TickSimulation()
    {
        ApprovedSimInput[] inputsForThisTick = _inputQueue.ToArray();
        _inputQueue.Clear();

        OnAboutToTickSimulation(inputsForThisTick);

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

        // NEW
        SimWorldUpdater.AvailableTicks.Add(tickData);

        // OLD
        SimulationView.Tick(tickData);
    }

    protected virtual void OnAboutToTickSimulation(ApprovedSimInput[] inputs) { }

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
                SimPlayerComponent unassignedSimPlayer = GetUnassignedSimPlayer();

                if (unassignedSimPlayer != null)
                {
                    PlayerRepertoireMaster.Instance.AssignSimPlayerToPlayer(playerInfo.PlayerId, unassignedSimPlayer.SimPlayerId);
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

    SimPlayerComponent GetUnassignedSimPlayer()
    {
        if (SimPlayerManager.Instance == null)
            return null;

        foreach (SimPlayerComponent simPlayer in SimulationView.EntitiesWithComponent<SimPlayerComponent>())
        {
            if (PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayer) == null)
            {
                return simPlayer;
            }
        }

        return null;
    }
}
