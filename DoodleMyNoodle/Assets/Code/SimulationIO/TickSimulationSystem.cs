using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SimulationControl
{
    [NetSerializable]
    public struct SimTickData : IEquatable<SimTickData>
    {
        public uint ExpectedNewTickId;
        public SimInputSubmission[] InputSubmissions;

        public bool Equals(SimTickData other)
        {
            return ReferenceEquals(InputSubmissions, other.InputSubmissions);
        }

        public SimInput[] ToSimInputArray()
        {
            SimInput[] array = new SimInput[InputSubmissions.Length];
            for (int i = 0; i < InputSubmissions.Length; i++)
            {
                array[i] = InputSubmissions[i].Input;
            }
            return array;
        }
    }

    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class TickSimulationSystem : ComponentSystem
    {
        public List<SimTickData> AvailableTicks = new List<SimTickData>();
        public bool IsTicking { get; private set; }
        public bool CanTick => _playSimulation.Evaluate()
            && _simulationLoadSceneSystem.OngoingSceneLoads.Count == 0;

        private Blocker _playSimulation = new Blocker();

        private SimulationWorld _simulationWorld;
        private SimulationWorldSystem _simulationWorldSystem;
        private LoadSimulationSceneSystem _simulationLoadSceneSystem;

        private SimPreInitializationSystemGroup _simPreInitGroup;
        private SimInitializationSystemGroup _simInitGroup;
        private SimSimulationSystemGroup _simSimGroup;
        private SimPresentationSystemGroup _simPresGroup;

        private ViewSystemGroup _viewGroup;

        bool _updatePlayerLoop;

        protected override void OnCreate()
        {
            base.OnCreate();

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _simulationLoadSceneSystem = World.GetOrCreateSystem<LoadSimulationSceneSystem>();
            _simulationWorld = _simulationWorldSystem.SimulationWorld;

            _simPreInitGroup = _simulationWorld.CreateSystem<SimPreInitializationSystemGroup>();
            _simInitGroup = _simulationWorld.CreateSystem<SimInitializationSystemGroup>();
            _simSimGroup = _simulationWorld.CreateSystem<SimSimulationSystemGroup>();
            _simPresGroup = _simulationWorld.CreateSystem<SimPresentationSystemGroup>();

            _simPreInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemEnd>());
            _simPreInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<UpdateSimulationTimeSystem>());

            _simInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
            _simInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());

            _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
            _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());
            foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimComponentSystem)))
            {
                _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem(systemType));
            }
            foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimJobComponentSystem)))
            {
                _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem(systemType));
            }

            _simPresGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());
            _simPresGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemBegin>());

            _simPreInitGroup.SortSystemUpdateList();
            _simInitGroup.SortSystemUpdateList();
            _simSimGroup.SortSystemUpdateList();
            _simPresGroup.SortSystemUpdateList();

            _viewGroup = World.GetOrCreateSystem<ViewSystemGroup>(); // system add & sort is done inside

            _updatePlayerLoop = true;


#if DEBUG_BUILD
            GameConsole.AddCommand("sim.pause", Cmd_SimPause, "Pause the simulation playback");
#endif
        }

        protected override void OnDestroy()
        {
#if DEBUG_BUILD
            GameConsole.RemoveCommand("sim.pause");
#endif
            base.OnDestroy();
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

                if (!CanTick)
                    break;

                IsTicking = true;

                SimTickData tick = AvailableTicks.First();
                AvailableTicks.RemoveAt(0);

                _simulationWorld.OngoingTickInputs = tick.ToSimInputArray();
                _simulationWorld.ExpectedNewTickId = tick.ExpectedNewTickId;

                ManualUpdate(_simPreInitGroup);
                ManualUpdate(_simInitGroup);
                ManualUpdate(_simSimGroup);
                ManualUpdate(_simPresGroup);

                ManualUpdate(_viewGroup);

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

        public void PauseSimulation(string key)
        {
            _playSimulation.BlockUnique(key);
        }

        public void UnpauseSimulation(string key)
        {
            _playSimulation.Unblock(key);
        }

        //public bool IsLocallySubmittedInputInQueue(InputSubmissionId inputSubmissionId)
        //{
        //    foreach (var tick in AvailableTicks)
        //    {
        //        foreach (var inputSubmissions in tick.InputSubmissions)
        //        {
        //            if (inputSubmissions.InstigatorConnectionId == uint.MaxValue && 
        //                inputSubmissions.ClientSubmissionId == inputSubmissionId)
        //                return true;
        //        }
        //    }

        //    return false;
        //}

#if DEBUG_BUILD
        private bool _isPausedByCmd = false;
        void Cmd_SimPause(string[] args)
        {
            if (_isPausedByCmd)
            {
                UnpauseSimulation(key: "cmd");
            }
            else
            {
                PauseSimulation(key: "cmd");
            }
        }
#endif
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


}