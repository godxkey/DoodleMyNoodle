using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class SimulationWorldUpdaterSystem : ComponentSystem
{
    public List<SimTickData> AvailableTicks = new List<SimTickData>();
    public bool IsTicking { get; private set; }

    private SimulationWorld _simulationWorld;
    private SimulationWorldSystem _simulationWorldSystem;
    private SimulationLoadSceneSystem _simulationLoadSceneSystem;
    
    private SimPreInitializationSystemGroup _preInitGroup;
    private SimInitializationSystemGroup _initGroup;
    private SimSimulationSystemGroup _simGroup;
    private SimPresentationSystemGroup _presGroup;

    bool _updatePlayerLoop;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
        _simulationLoadSceneSystem = World.GetOrCreateSystem<SimulationLoadSceneSystem>();
        _simulationWorld = _simulationWorldSystem.SimulationWorld;

        _preInitGroup = _simulationWorld.CreateSystem<SimPreInitializationSystemGroup>();
        _initGroup = _simulationWorld.CreateSystem<SimInitializationSystemGroup>();
        _simGroup = _simulationWorld.CreateSystem<SimSimulationSystemGroup>();
        _presGroup = _simulationWorld.CreateSystem<SimPresentationSystemGroup>();

        _preInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemEnd>());
        _preInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<UpdateSimulationTimeSystem>());

        _initGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
        _initGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());

        _simGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
        _simGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimComponentSystem)))
        {
            _simGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem(systemType));
        }
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimJobComponentSystem)))
        {
            _simGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem(systemType));
        }

        _presGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());
        _presGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemBegin>());

        _preInitGroup.SortSystemUpdateList();
        _initGroup.SortSystemUpdateList();
        _simGroup.SortSystemUpdateList();
        _presGroup.SortSystemUpdateList();

        _updatePlayerLoop = true;
    }

    protected override void OnUpdate()
    {
        if (_updatePlayerLoop)
        {
            // !! IMPORTANT !!
            // Even if we update the simulation manually, we have to add our ComponentSystemGroups to the player loop if we want them to
            // show up in the EntityDebugger window.
            ScriptBehaviourUpdateOrderEx.AddWorldSystemGroupsIntoPlayerLoop(_simulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
            _updatePlayerLoop = false;
        }


        while (AvailableTicks.Count > 0)
        {
           _simulationLoadSceneSystem.UpdateSceneLoading();

            if (_simulationLoadSceneSystem.OngoingSceneLoads.Count > 0)
                break;

            IsTicking = true;

            SimTickData tick = AvailableTicks.First();
            AvailableTicks.RemoveAt(0);

            _simulationWorld.OngoingTickInputs = tick.inputs;

            ManualUpdate(_preInitGroup);
            ManualUpdate(_initGroup);
            ManualUpdate(_simGroup);
            ManualUpdate(_presGroup);

            _simulationWorld.OngoingTickInputs = null;

            IsTicking = false;
        }
    }

    private void ManualUpdate(IManualSystemGroupUpdate systemGroup)
    {
        systemGroup.CanUpdate = true;
        systemGroup.Update();
        systemGroup.CanUpdate = false;
    }

#if !UNITY_DOTSPLAYER
    static class ScriptBehaviourUpdateOrderEx
    {
        /// <summary>
        /// Update the player loop with a world's root-level systems
        /// </summary>
        /// <param name="world">World with root-level systems that need insertion into the player loop</param>
        /// <param name="existingPlayerLoop">Optional parameter to preserve existing player loops (e.g. ScriptBehaviourUpdateOrder.CurrentPlayerLoop)</param>
        public static void AddWorldSystemGroupsIntoPlayerLoop(World world, PlayerLoopSystem? existingPlayerLoop = null)
        {
            PlayerLoopSystem playerLoop = existingPlayerLoop ?? PlayerLoop.GetDefaultPlayerLoop();

            if (world != null)
            {
                // Insert the root-level systems into the appropriate PlayerLoopSystem subsystems:
                for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
                {
                    if (playerLoop.subSystemList[i].type == typeof(Update))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            AddSystem<SimSimulationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList);
                    }
                    else if (playerLoop.subSystemList[i].type == typeof(PreLateUpdate))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            AddSystem<SimPresentationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList);
                    }
                    else if (playerLoop.subSystemList[i].type == typeof(Initialization))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            AddSystem<SimPreInitializationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList);

                        playerLoop.subSystemList[i].subSystemList =
                            AddSystem<SimInitializationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList);
                    }
                }
            }

            ScriptBehaviourUpdateOrder.SetPlayerLoop(playerLoop);
        }

        static PlayerLoopSystem[] AddSystem<T>(World world, PlayerLoopSystem[] oldArray)
            where T : ComponentSystemBase
        {
            return InsertSystem<T>(world, oldArray, oldArray.Length);
        }
        static PlayerLoopSystem[] InsertSystem<T>(World world, PlayerLoopSystem[] oldArray, int insertIndex)
            where T : ComponentSystemBase
        {
            T system = world.GetExistingSystem<T>();
            if (system == null)
                return oldArray;

            var newArray = new PlayerLoopSystem[oldArray.Length + 1];

            int o = 0;
            for (int n = 0; n < newArray.Length; ++n)
            {
                if (n == insertIndex)
                {
                    continue;
                }

                newArray[n] = oldArray[o];
                ++o;

            }
            ScriptBehaviourUpdateOrder.InsertManagerIntoSubsystemList<T>(newArray, insertIndex, world.GetOrCreateSystem<T>());

            return newArray;
        }
    }
#endif
}

