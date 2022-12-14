using CCC.Fix2D;
using CCC.Operations;
using Sim.Operations;
using SimulationControl;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngineX;

public class ExternalSimGameWorldAccessor : ExternalSimWorldAccessor
{
    public PresentationEventsWithReadAccess PresentationEvents => (PresentationEventsWithReadAccess)((InternalSimGameWorldAccessor)((SimulationGameWorld)GetSimWorld()).GetInternalAccessor()).PresentationEvents;
}

public partial class SimulationController : GameSystem<SimulationController>
{
    private TickSimulationSystem _tickSystem;

    public static void Initialize()
    {
        SimulationWorldSystem.WorldInstantiationFunc = (name) => new SimulationGameWorld(name);
        SimulationWorldSystem.ExternalSimWorldAccessorInstantiationFunc = () => new ExternalSimGameWorldAccessor();
    }

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        SimSerializationOperation.BlobAssetDataCollectors[typeof(PhysicsColliderBlob)] = new BlobPhysicsColliderCollector();
        SimDeserializationOperation.BlobAssetDataDistributors[typeof(PhysicsColliderBlob)] = new BlobPhysicsColliderDistributor();

        Time.fixedDeltaTime = (float)SimulationConstants.TIME_STEP;

        World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<SimulationControlSystemGroup>()
            .Initialize(OnlineService.OnlineInterface?.SessionInterface, ValidateSimInput);

        _tickSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<TickSimulationSystem>();
        _tickSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<TickSimulationSystem>();
        _tickSystem.PauseSimulation("game-not-ready");
        _tickSystem.SimulationTicked += OnSimulationTicked;
        _tickSystem.UpdateFinished += OnTickUpdateFinished;
    }

    public override void OnGameStart()
    {
        base.OnGameStart();
        _tickSystem.ShouldUpdateView = true;
        _tickSystem.UnpauseSimulation("game-not-ready");
    }

    private void OnSimulationTicked(SimTickData tickData)
    {
        PresentationEventsWithReadAccess.ShouldUseSinceLastTick = true;
        Profiler.BeginSample("SimulationController.PostSimulationTick It");
        foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
        {
#if DEBUG
            try
            {
#endif
                if (b is IPresentationPostSimTick p && b.isActiveAndEnabled)
                {
                    Profiler.BeginSample(b.ProfilingMarkerPostSimTick);
                    p.PresentationPostSimulationTick();
                    Profiler.EndSample();
                }
#if DEBUG
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " - stack:\n " + e.StackTrace);
            }
#endif
        }
        Profiler.EndSample();
        PresentationEventsWithReadAccess.ShouldUseSinceLastTick = false;
    }

    private void OnTickUpdateFinished()
    {
        if (!GamePresentationCache.Instance.Ready)
            return;

        Profiler.BeginSample("SimulationController.PresentationUpdate It");
        foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
        {
#if DEBUG
            try
            {
#endif
                if (b is IPresentationUpdate p && b.isActiveAndEnabled)
                {
                    Profiler.BeginSample(b.ProfilingMarkerPresentationUpdate);
                    p.PresentationUpdate();
                    Profiler.EndSample();
                }
#if DEBUG
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " - stack:\n " + e.StackTrace);
            }
#endif
        }
        Profiler.EndSample();
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

        if (World.DefaultGameObjectInjectionWorld?.GetExistingSystem<TickSimulationSystem>() != null)
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<TickSimulationSystem>().SimulationTicked -= OnSimulationTicked;
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<TickSimulationSystem>().UpdateFinished -= OnTickUpdateFinished;
        }

        World.DefaultGameObjectInjectionWorld?.GetExistingSystem<SimulationControlSystemGroup>()?.Shutdown();
    }
}
