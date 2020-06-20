/*using SimulationControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;

public class SimulationControllerMaster : SimulationController
{
    //[ConfigVar("sim.tick_rateold", "1", description: "The number of ticks executed per fixed update")]
    //static ConfigVar s_tickRate;

    //private float _tickRateCounter;

    //Queue<ApprovedSimInput> _inputQueue = new Queue<ApprovedSimInput>();

    //// called by local player
    //public override void SubmitInput(SimInput input)
    //{
    //    if (input == null)
    //    {
    //        Log.Error("Trying to submit a null input");
    //        return;
    //    }

    //    PlayerInfo localPlayer = PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
    //    QueueInput(input, localPlayer, new InputSubmissionId(0));
    //}

    //protected void QueueInput(SimInput input, PlayerInfo playerInfo, InputSubmissionId submissionId)
    //{
    //    if (!CanTickSimulation) // we don't accept any input while sim is paused
    //        return;

    //    if (input is SimPlayerInput playerInput)
    //    {
    //        if (playerInfo.SimPlayerId != PersistentId.Invalid)
    //        {
    //            // Set SimPlayerId in input
    //            // fbessette: Currently, since the 'SimPlayerId' is a field in the inputs, the clients pointlessly have that data sent to us.
    //            //            To optimize the package's size, we could refactor this a little and pull the SimPlayerId out of the base input data class
    //            playerInput.SimPlayerId = playerInfo.SimPlayerId;
    //        }
    //        else
    //        {
    //            Log.Info($"[{nameof(SimulationControllerMaster)}] We refused {playerInfo.PlayerName}'s input because he doesn't have a " +
    //                $"valid SimPlayerId yet.");
    //            return; // player cannot submit inputs yet
    //        }
    //    }

    //    _inputQueue.Enqueue(new ApprovedSimInput()
    //    {
    //        input = input,
    //        playerInstigator = playerInfo.PlayerId,
    //        clientSubmissionId = submissionId
    //    });
    //}

    //void FixedUpdate()
    //{
    //    if (!Game.Started)
    //        return;

    //    SimulationView.UpdateSceneLoads();

    //    _tickRateCounter += s_tickRate.FloatValue;

    //    while (_tickRateCounter > 0)
    //    {
    //        if (CanTickSimulation)
    //        {
    //            AssignSimPlayerIdsToPlayersMissingOne();

    //            TickSimulation();
    //        }
    //        else
    //        {
    //            _inputQueue.Clear();
    //        }

    //        _tickRateCounter--;
    //    }
    //}

    //void TickSimulation()
    //{
    //    ApprovedSimInput[] inputsForThisTick = _inputQueue.ToArray();
    //    _inputQueue.Clear();

    //    OnAboutToTickSimulation(inputsForThisTick);

    //    // Tick the simulation locally
    //    SimInput[] simInputs = new SimInput[inputsForThisTick.Length];
    //    for (int i = 0; i < inputsForThisTick.Length; i++)
    //    {
    //        simInputs[i] = inputsForThisTick[i].input;
    //    }

    //    SimTickData tickData = new SimTickData()
    //    {
    //        inputs = simInputs
    //    };

    //    // OLD
    //    SimulationView.Tick(tickData);
    //}

    //protected virtual void OnAboutToTickSimulation(ApprovedSimInput[] inputs) { }

    //void AssignSimPlayerIdsToPlayersMissingOne()
    //{
    //    // fbessette: For now, we assign SimPlayers randomly. First player arrived gets the first SimPlayer
    //    //            Eventually, we'll want returning players to get back their old SimPlayer to keep their character/gear/etc.

    //    foreach (PlayerInfo playerInfo in PlayerRepertoireSystem.Instance.Players)
    //    {
    //        if(PlayerIdHelpers.GetSimPlayerFromPlayer(playerInfo, Game.SimulationWorld) == Entity.Null)
    //        {
    //            Entity unassignedPlayer = GetUnassignedSimPlayer();

    //            if (unassignedPlayer == Entity.Null)
    //            {
    //                // ask the simulation to create a new player
    //                SubmitInput(new SimInputPlayerCreate() { PlayerName = playerInfo.PlayerName }); 
    //            }
    //            else
    //            {
    //                PersistentId simPlayerId = Game.SimulationWorld.EntityManager.GetComponentData<PersistentId>(unassignedPlayer);

    //                PlayerRepertoireMaster.Instance.AssignSimPlayerToPlayer(playerInfo.PlayerId, simPlayerId);
    //            }
    //        }
    //    }
    //}

    //Entity GetUnassignedSimPlayer()
    //{
    //    World simWorld = Game.SimulationWorld;
    //    if (simWorld == null)
    //        return Entity.Null;


    //    using (EntityQuery query = simWorld.EntityManager.CreateEntityQuery(
    //        ComponentType.ReadOnly<PlayerTag>(),
    //        ComponentType.ReadOnly<PersistentId>()))
    //    {
    //        using (NativeArray<Entity> simPlayers = query.ToEntityArray(Allocator.TempJob))
    //        {
    //            foreach (Entity playerEntity in simPlayers)
    //            {
    //                if (PlayerIdHelpers.GetPlayerFromSimPlayer(playerEntity, simWorld) == null)
    //                {
    //                    return playerEntity;
    //                }
    //            }
    //        }
    //    }


    //    return Entity.Null;
    //}
}
*/