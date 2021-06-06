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
        public bool RepackEntities; // if true, the tick system should serialize->deserialize the world before ticking
        public bool ChecksumAfter;

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
        public bool ShouldUpdateView { get; set; } = false;
        public bool CanTick => _playSimulation
            && _simulationLoadSceneSystem.OngoingSceneLoads.Count == 0
            && _inPlayerLoop;

        public event Action<SimTickData> SimulationTicked;

        private DisablableValue _playSimulation = new DisablableValue();

        private SimulationWorldSystem _simulationWorldSystem;
        private LoadSimulationSceneSystem _simulationLoadSceneSystem;

        private SimPreInitializationSystemGroup _simPreInitGroup;
        private SimInitializationSystemGroup _simInitGroup;
        private SimSimulationSystemGroup _simSimGroup;
        private SimPresentationSystemGroup _simPresGroup;
        private SimPostPresentationSystemGroup _simPostPresGroup;
        private ViewSystemGroup _viewGroup;
        private bool _inPlayerLoop;
        private (RepackSimulationOperation op, uint tick, bool worldReplaceRequested) _repack;

        private enum RepackSteps
        {
            None,
            InProgress,
            ReplacementRequested
        }

        private static IEnumerable<Type> s_simSystemTypes;
        public static IEnumerable<Type> AllSimSystemTypes
        {
            get
            {
                if (s_simSystemTypes is null)
                {
                    s_simSystemTypes = TypeUtility.GetTypesDerivedFrom(typeof(ISimSystem)).Where(t => !t.IsAbstract);
                }
                return s_simSystemTypes;
            }
        }

        private bool _addToPlayerLoop;

        protected override void OnCreate()
        {
            base.OnCreate();

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _simulationLoadSceneSystem = World.GetOrCreateSystem<LoadSimulationSceneSystem>();

            _viewGroup = World.GetOrCreateSystem<ViewSystemGroup>(); // system add & sort is done inside

#if DEBUG
            s_commandInstance = this;
            GameConsole.SetCommandOrVarEnabled("Sim.Pause", true);
#endif
        }

        protected override void OnDestroy()
        {
#if DEBUG
            GameConsole.SetCommandOrVarEnabled("Sim.Pause", false);
            s_commandInstance = null;
#endif
            base.OnDestroy();
        }

        public void RemoveFromPlayerLoop(World simWorld)
        {
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(simWorld);
            _inPlayerLoop = false;
            _addToPlayerLoop = false;
        }

        public void CreateSimSystemsAndAddToPlayerLoop(World simWorld)
        {
            _simPreInitGroup = simWorld.CreateSystem<SimPreInitializationSystemGroup>();
            _simInitGroup = simWorld.CreateSystem<SimInitializationSystemGroup>();
            _simSimGroup = simWorld.CreateSystem<SimSimulationSystemGroup>();
            _simPresGroup = simWorld.CreateSystem<SimPresentationSystemGroup>();
            _simPostPresGroup = simWorld.CreateSystem<SimPostPresentationSystemGroup>();

            // pre init group (not visible in EntityDebugger for some reason ...)
            _simPreInitGroup.AddSystemToUpdateList(simWorld.CreateSystem<ChangeDetectionSystemEnd>());
            _simPreInitGroup.AddSystemToUpdateList(simWorld.CreateSystem<UpdateSimulationTimeSystem>());
            _simPreInitGroup.AddSystemToUpdateList(simWorld.CreateSystem<InitializeRandomSeedSystem>());

            // init group
            _simInitGroup.AddSystemToUpdateList(simWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
            _simInitGroup.AddSystemToUpdateList(simWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());

            // sim group
            _simSimGroup.AddSystemToUpdateList(simWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
            CCC.Fix2D.Fix2DHelpers.AddPhysicsSystemsToGroup(simWorld, _simSimGroup);
            _simSimGroup.AddSystemToUpdateList(simWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());

            // pres group
            _simPresGroup.AddSystemToUpdateList(simWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());

            // post pres
            _simPostPresGroup.AddSystemToUpdateList(simWorld.CreateSystem<ChangeDetectionSystemBegin>());

            AddSystemsToRootLevelSystemGroups(simWorld, AllSimSystemTypes);

            _addToPlayerLoop = true;
        }

        protected override void OnUpdate()
        {
            if (_addToPlayerLoop)
            {
                // !! IMPORTANT !!
                // Even if we update the simulation manually, we have to add our ComponentSystemGroups to the player loop if we want them to
                // show up in the EntityDebugger window.
                // TODO: remove old world from player loop
                ScriptBehaviourUpdateOrderEx.AddWorldToCurrentPlayerLoop(_simulationWorldSystem.SimulationWorld);
                _inPlayerLoop = true;
                _addToPlayerLoop = false;
            }

            bool tickedThisFrame = false;

            while (AvailableTicks.Count > 0)
            {
                SimTickData tick = AvailableTicks.First();

                if (UpdateWorldRepack(tick))
                    break;

                _simulationLoadSceneSystem.UpdateSceneLoading();

                if (!CanTick)
                    break;

                tickedThisFrame = true;

                // Consume tick!
                AvailableTicks.RemoveAt(0);

                Log.Info(SimulationIO.LogChannel, $"Begin sim tick '{tick.ExpectedNewTickId}' with {tick.InputSubmissions.Count} inputs.");

                _simulationWorldSystem.HasJustRepacked = false;
                _simulationWorldSystem.SimulationWorld.TickInputs = tick.ToSimInputArray();
                _simulationWorldSystem.SimulationWorld.ExpectedNewTickId = tick.ExpectedNewTickId;

                ManualUpdate(_simPreInitGroup);
                ManualUpdate(_simInitGroup);
                ManualUpdate(_simSimGroup);
                ManualUpdate(_simPresGroup);
                ManualUpdate(_simPostPresGroup);

                Log.Info(SimulationIO.LogChannel, $"End sim tick '{tick.ExpectedNewTickId}'");

                if (ShouldUpdateView)
                {
                    ManualUpdate(_viewGroup);
                }

                // this ensures our previously scheduled view jobs are done (we might want to find a more performant alternative)
                World.EntityManager.CompleteAllJobs();

                SimulationTicked?.Invoke(tick);

                _simulationWorldSystem.SimulationWorld.TickInputs = null;
            }

            if (!tickedThisFrame && ShouldUpdateView)
            {
                ManualUpdate(_viewGroup);

                // this ensures our previously scheduled view jobs are done (we might want to find a more performant alternative)
                World.EntityManager.CompleteAllJobs();
            }
        }

        private bool UpdateWorldRepack(SimTickData tick)
        {
            // if the tick does not mention repack, we do not need to repack
            if (!tick.RepackEntities)
                return false;

            // If we already repacked last tick, no need to do it again
            if (_simulationWorldSystem.HasJustRepacked)
                return false;

            // start operation if not already ongoing
            if (_repack.op == null || _repack.tick != tick.ExpectedNewTickId)
            {
                //Log.Warning("new RepackSimulationOperation()");
                var newWorld = _simulationWorldSystem.CreateNewReplacementWorld();
                var op = new RepackSimulationOperation(_simulationWorldSystem.SimulationWorld, newWorld);
                var tickId = tick.ExpectedNewTickId;
                var worldReplaceRequested = false;
                
                _repack = (op, tickId, worldReplaceRequested);
                _repack.op.Execute();
            }

            if (_repack.op.IsDone && !_repack.worldReplaceRequested)
            {
                if (_repack.op.HasSucceeded)
                {
                    DebugScreenMessage.DisplayMessage("Repack successful");
                    _simulationWorldSystem.RequestReplaceSimWorld((SimulationWorld)_repack.op.NewWorld);
                }
                else
                {
                    DebugScreenMessage.DisplayMessage("Repack failed");
                    Log.Error($"Failed to repack before tick {tick.ExpectedNewTickId}. Simulation will continue normally, but entities might be desynced from server.");
                    
                    // remove the 'Repack' flag from the first tick to let simulation continue
                    if (AvailableTicks.Count > 0)
                    {
                        var t = AvailableTicks[0];
                        t.RepackEntities = false;
                        AvailableTicks[0] = t;
                    }
                }

                _repack.worldReplaceRequested = true;
                //Log.Warning("_repackOperation = null;");
            }

            return true; // means "cannot tick"
        }

        private void ManualUpdate(IManualSystemGroupUpdate systemGroup)
        {
            systemGroup.CanUpdate = true;
            systemGroup.Update();
            systemGroup.CanUpdate = false;
        }

        public void PauseSimulation(string key)
        {
            _playSimulation.AddUniqueDisable(key);
        }

        public void UnpauseSimulation(string key)
        {
            _playSimulation.RemoveDisable(key);
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
            initializationSystemGroup.SortSystems();
            simulationSystemGroup.SortSystems();
            presentationSystemGroup.SortSystems();
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

        [ConsoleCommand("Sim.Pause", "Pause the simulation playback", EnabledByDefault = false)]
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

#if !UNITY_DOTSRUNTIME
        static class ScriptBehaviourUpdateOrderEx
        {
            public static void AddWorldToCurrentPlayerLoop(World world)
            {
                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                AddWorldToPlayerLoop(world, ref playerLoop);
                PlayerLoop.SetPlayerLoop(playerLoop);
            }

            public static void AddWorldToPlayerLoop(World world, ref PlayerLoopSystem playerLoop)
            {
                if (world == null)
                    return;

                var initGroup = world.GetExistingSystem<SimInitializationSystemGroup>();
                if (initGroup != null)
                    ScriptBehaviourUpdateOrder.AppendSystemToPlayerLoopList(initGroup, ref playerLoop, typeof(Initialization));

                var simGroup = world.GetExistingSystem<SimSimulationSystemGroup>();
                if (simGroup != null)
                    ScriptBehaviourUpdateOrder.AppendSystemToPlayerLoopList(simGroup, ref playerLoop, typeof(Update));

                var presGroup = world.GetExistingSystem<SimPresentationSystemGroup>();
                if (presGroup != null)
                    ScriptBehaviourUpdateOrder.AppendSystemToPlayerLoopList(presGroup, ref playerLoop, typeof(PreLateUpdate));
            }
        }
#endif
    }


}