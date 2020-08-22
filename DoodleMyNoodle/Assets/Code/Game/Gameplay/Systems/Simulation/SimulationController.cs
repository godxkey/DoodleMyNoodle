using CCC.Operations;
using SimulationControl;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class SimulationController : GameSystem<SimulationController>
{
    public override bool SystemReady => true;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        Time.fixedDeltaTime = (float)SimulationConstants.TIME_STEP;
        
        World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<SimulationControlSystemGroup>()
            .Initialize(OnlineService.OnlineInterface?.SessionInterface, ValidateSimInput);

        World.DefaultGameObjectInjectionWorld
        .GetExistingSystem<TickSimulationSystem>()
        .SimulationTicked += OnSimulationTicked;
    }

    private void OnSimulationTicked(SimTickData tickData)
    {
        foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
        {
#if DEBUG
            try
            {
#endif
                if (b is IPostSimulationTick p)
                    p.OnPostSimulationTick();
#if DEBUG
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " - stack:\n " + e.StackTrace);
            }
#endif
        }
    }

    private bool ValidateSimInput(SimInput input, INetworkInterfaceConnection instigator)
    {
        PlayerInfo instigatorPlayer = PlayerHelpers.GetPlayerInfo(instigator);

        if (instigatorPlayer == null)
        {
            Log.Info($"[{nameof(SimulationController)}] We refused an input from connection {instigator.Id} " +
                $"because no player seems associated with that connection.");
            return false;
        }

        if (input is SimPlayerInput playerInput)
        {
            playerInput.SimPlayerId = instigatorPlayer.SimPlayerId;

            if (playerInput.SimPlayerId == PersistentId.Invalid)
            {
                Log.Info($"[{nameof(SimulationController)}] We refused {instigatorPlayer.PlayerName}'s input " +
                    $"because (s)he doesn't have a valid {nameof(instigatorPlayer.SimPlayerId)} yet.");
                return false;
            }
        }

        return true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(World.DefaultGameObjectInjectionWorld?.GetExistingSystem<TickSimulationSystem>() != null)
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<TickSimulationSystem>().SimulationTicked -= OnSimulationTicked;
        }

        World.DefaultGameObjectInjectionWorld?.GetExistingSystem<SimulationControlSystemGroup>()?.Shutdown();
    }
}
