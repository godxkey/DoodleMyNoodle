using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngineX;

namespace SimulationControl
{
    [NetSerializable]
    public struct SimTickData : IEquatable<SimTickData>
    {
        public uint ExpectedNewTickId;
        public List<SimInputSubmission> InputSubmissions;

        public bool Equals(SimTickData other)
        {
            return ReferenceEquals(InputSubmissions, other.InputSubmissions);
        }

        public SimInput[] ToSimInputArray()
        {
            SimInput[] array = new SimInput[InputSubmissions.Count];
            for (int i = 0; i < InputSubmissions.Count; i++)
            {
                array[i] = InputSubmissions[i].Input;
            }
            return array;
        }

        public override string ToString()
        {
            return $"tick({ExpectedNewTickId}:{InputSubmissions.Count})";
        }
    }

    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class TickSimulationSystem : ComponentSystem
    {
        public List<SimTickData> AvailableTicks = new List<SimTickData>();
        public bool IsTicking { get; private set; }
        public bool CanTick => _playSimulation.Evaluate()
            && _simulationLoadSceneSystem.OngoingSceneLoads.Count == 0;

        public event Action<SimTickData> SimulationTicked;

        private Blocker _playSimulation = new Blocker();

        private SimulationWorld _simulationWorld;
        private SimulationWorldSystem _simulationWorldSystem;
        private LoadSimulationSceneSystem _simulationLoadSceneSystem;

        private SimPreInitializationSystemGroup _simPreInitGroup;
        private SimInitializationSystemGroup _simInitGroup;
        private SimSimulationSystemGroup _simSimGroup;
        private SimPresentationSystemGroup _simPresGroup;
        private SimPostPresentationSystemGroup _simPostPresGroup;
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
            _simPostPresGroup = _simulationWorld.CreateSystem<SimPostPresentationSystemGroup>();

            // pre init group (not visible in EntityDebugger for some reason ...)
            _simPreInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemEnd>());
            _simPreInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<UpdateSimulationTimeSystem>());
            _simPreInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<InitializeRandomSeedSystem>());

            // init group
            _simInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
            _simInitGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());

            // sim group
            _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
            _simSimGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());

            // pres group
            _simPresGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());

            // post pres
            _simPostPresGroup.AddSystemToUpdateList(_simulationWorld.CreateSystem<ChangeDetectionSystemBegin>());


            var simSystemTypes = TypeUtility.GetTypesDerivedFrom(typeof(ISimSystem)).Where(t => !t.IsAbstract);

            AddSystemsToRootLevelSystemGroups(_simulationWorld, simSystemTypes);

            //_simPreInitGroup.SortSystemUpdateList();
            //_simInitGroup.SortSystemUpdateList();
            //_simSimGroup.SortSystemUpdateList();
            //_simPresGroup.SortSystemUpdateList();

            _viewGroup = World.GetOrCreateSystem<ViewSystemGroup>(); // system add & sort is done inside

            _updatePlayerLoop = true;


#if DEBUG
            s_commandInstance = this;
            GameConsole.SetCommandOrVarEnabled("sim.pause", true);
#endif
        }

        protected override void OnDestroy()
        {
#if DEBUG
            GameConsole.SetCommandOrVarEnabled("sim.pause", false);
            s_commandInstance = null;
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
                ScriptBehaviourUpdateOrderEx.AddWorldSystemGroupsIntoPlayerLoop(_simulationWorld, PlayerLoop.GetCurrentPlayerLoop());
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

                Log.Info(SimulationIO.LogChannel, $"Begin sim tick '{tick.ExpectedNewTickId}' with {tick.InputSubmissions.Count} inputs.");

                _simulationWorld.TickInputs = tick.ToSimInputArray();
                _simulationWorld.ExpectedNewTickId = tick.ExpectedNewTickId;

                ManualUpdate(_simPreInitGroup);
                ManualUpdate(_simInitGroup);
                ManualUpdate(_simSimGroup);
                ManualUpdate(_simPresGroup);
                ManualUpdate(_simPostPresGroup);

                Log.Info(SimulationIO.LogChannel, $"End sim tick '{tick.ExpectedNewTickId}'");

                ManualUpdate(_viewGroup);

                // this ensures our previously scheduled view jobs are done (we might want to find a more performant alternative)
                World.EntityManager.CompleteAllJobs();

                SimulationTicked?.Invoke(tick);

                _simulationWorld.TickInputs = null;

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

        // fbessette: This code mostly comes from the DefaultWorldInitialization
        /// <summary>
        /// Adds the collection of systems to the world by injecting them into the root level system groups
        /// (InitializationSystemGroup, SimulationSystemGroup and PresentationSystemGroup)
        /// </summary>
        private static void AddSystemsToRootLevelSystemGroups(World world, IEnumerable<Type> systems)
        {
            // create presentation system and simulation system
            var initializationSystemGroup = world.GetOrCreateSystem<SimInitializationSystemGroup>();
            var simulationSystemGroup = world.GetOrCreateSystem<SimSimulationSystemGroup>();
            var presentationSystemGroup = world.GetOrCreateSystem<SimPresentationSystemGroup>();

            // Add systems to their groups, based on the [UpdateInGroup] attribute.
            foreach (var type in systems)
            {
                // Skip the built-in root-level system groups
                if (type == typeof(SimInitializationSystemGroup) ||
                    type == typeof(SimSimulationSystemGroup) ||
                    type == typeof(SimPresentationSystemGroup))
                {
                    continue;
                }

                var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);
                if (groups.Length == 0)
                {
                    simulationSystemGroup.AddSystemToUpdateList(GetOrCreateSystemAndLogException(world, type));
                }

                foreach (var g in groups)
                {
                    var group = g as UpdateInGroupAttribute;
                    if (group == null)
                        continue;

                    if (!(typeof(ComponentSystemGroup)).IsAssignableFrom(group.GroupType))
                    {
                        Debug.LogError($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} must be derived from ComponentSystemGroup.");
                        continue;
                    }

                    if (type == typeof(SimPreInitializationSystemGroup) ||
                        type == typeof(SimPostPresentationSystemGroup))
                    {
                        Debug.LogError($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} is reserved for internal stuff.");
                        continue;
                    }

                    // Warn against unexpected behaviour combining DisableAutoCreation and UpdateInGroup
                    var parentDisableAutoCreation = Attribute.IsDefined(group.GroupType, typeof(DisableAutoCreationAttribute));
                    if (parentDisableAutoCreation)
                    {
                        Debug.LogWarning($"A system {type} wants to execute in {group.GroupType} but this group has [DisableAutoCreation] and {type} does not.");
                    }

                    var groupSys = GetOrCreateSystemAndLogException(world, group.GroupType) as ComponentSystemGroup;
                    if (groupSys == null)
                    {
                        Debug.LogWarning(
                            $"Skipping creation of {type} due to errors creating the group {group.GroupType}. Fix these errors before continuing.");
                        continue;
                    }

                    groupSys.AddSystemToUpdateList(GetOrCreateSystemAndLogException(world, type));
                }
            }

            // Update player loop
            initializationSystemGroup.SortSystemUpdateList();
            simulationSystemGroup.SortSystemUpdateList();
            presentationSystemGroup.SortSystemUpdateList();
        }

        static ComponentSystemBase GetOrCreateSystemAndLogException(World world, Type type)
        {
            try
            {
                if (type == typeof(InitializationSystemGroup))
                    type = typeof(SimInitializationSystemGroup);

                if (type == typeof(SimulationSystemGroup))
                    type = typeof(SimSimulationSystemGroup);

                if (type == typeof(PresentationSystemGroup))
                    type = typeof(SimPresentationSystemGroup);

                return world.GetOrCreateSystem(type);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

#if DEBUG
        private bool _isPausedByCmd = false;
        private static TickSimulationSystem s_commandInstance;
        [ConsoleCommand("sim.pause", "Pause the simulation playback", EnabledByDefault = false)]
        private static void Cmd_SimPause()
        {
            if (s_commandInstance._isPausedByCmd)
            {
                s_commandInstance.UnpauseSimulation(key: "cmd");
                s_commandInstance._isPausedByCmd = false;
            }
            else
            {
                s_commandInstance.PauseSimulation(key: "cmd");
                s_commandInstance._isPausedByCmd = true;
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
                            playerLoop.subSystemList[i].subSystemList = AddSystem<SimSimulationSystemGroup>(world, playerLoop.subSystemList[i].subSystemList);
                        }
                        else if (playerLoop.subSystemList[i].type == typeof(PreLateUpdate))
                        {
                            playerLoop.subSystemList[i].subSystemList = AddSystem<SimPresentationSystemGroup>(world, playerLoop.subSystemList[i].subSystemList);
                            playerLoop.subSystemList[i].subSystemList = AddSystem<SimPostPresentationSystemGroup>(world, playerLoop.subSystemList[i].subSystemList);
                        }
                        else if (playerLoop.subSystemList[i].type == typeof(Initialization))
                        {
                            playerLoop.subSystemList[i].subSystemList = AddSystem<SimPreInitializationSystemGroup>(world, playerLoop.subSystemList[i].subSystemList);
                            playerLoop.subSystemList[i].subSystemList = AddSystem<SimInitializationSystemGroup>(world, playerLoop.subSystemList[i].subSystemList);
                        }
                    }
                }

                PlayerLoop.SetPlayerLoop(playerLoop);
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