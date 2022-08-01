using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine.AI;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngineX;
using UnityX.EntitiesX.SerializationX;

namespace SimulationControl
{
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SimulationWorldSystem : ComponentSystem, IWorldOwner
    {
        private List<string> _incomingEntityInjections = new List<string>();
        private TickSimulationSystem _tickSystem;

        World IWorldOwner.OwnedWorld => SimulationWorld;
        internal SimulationWorld SimulationWorld { get; private set; }
        public World PresentationWorld => World;
        public ExternalSimWorldAccessor SimWorldAccessor { get; private set; }
        public bool ReadyForEntityInjections { get; set; } = false;

        public event Action WorldReplaced;
        public uint ReplaceVersion { get; private set; }
        public bool HasJustRepacked { get; set; }

        public static Func<string, SimulationWorld> WorldInstantiationFunc = (name) => new SimulationWorld(name);
        public static Func<ExternalSimWorldAccessor> ExternalSimWorldAccessorInstantiationFunc = () => new ExternalSimWorldAccessor();

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

        protected override void OnCreate()
        {
            base.OnCreate();

            SimWorldAccessor = ExternalSimWorldAccessorInstantiationFunc();
            World.GetOrCreateSystem<LoadSimulationSceneSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();

            RequestReplaceSimWorld(CreateNewReplacementWorld());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //_tickSystem.RemoveFromPlayerLoop(SimulationWorld);
            if (SimulationWorld.IsCreated)
                SimulationWorld.Dispose();

            EntityDebugProxy.FavorWorldInDebugging = null;

            SimWorldAccessor.SimWorld = null;
            SimWorldAccessor.EntityManager = default;
            SimWorldAccessor.SomeSimSystem = null;

            SimulationWorld = null;
        }

        protected override void OnUpdate()
        {
            if (_replaceWorld != null)
            {
                if (_inPlayerLoop) // Remove simworld from player loop 1 frame in advance to avoid unity errors ...
                {
                    if (SimulationWorld != null)
                    {
                        _tickSystem.RemoveFromPlayerLoop(SimulationWorld);
                    }

                    _inPlayerLoop = false;
                }
                else
                {
                    ReplaceSimWorld(_replaceWorld);
                    _replaceWorld = null;
                }
            }
        }

        public SimulationWorld CreateNewReplacementWorld()
        {
            var newWorld = WorldInstantiationFunc($"Simulation World (replace ver:{ReplaceVersion})");
            CreateSimSystems(newWorld);
            return newWorld;
        }

        public void RequestReplaceSimWorld(SimulationWorld newWorld)
        {
            _replaceWorld = newWorld;
        }

        private void ReplaceSimWorld(SimulationWorld newWorld)
        {
            Log.Info($"Replacing sim world with new one (at tick {newWorld.GetLastTickIdFromEntity()})");
            if (SimulationWorld != null)
            {
                if (_inPlayerLoop)
                {
                    _tickSystem.RemoveFromPlayerLoop(SimulationWorld);
                    _inPlayerLoop = false;
                }
                SimulationWorld.Dispose();
                SimulationWorld = null;
            }

            EntityDebugProxy.FavorWorldInDebugging = newWorld;
            SimulationWorld = newWorld;

            newWorld.Owner = this;

            _tickSystem.AddToPlayerLoop(newWorld);
            _inPlayerLoop = true;

            SimWorldAccessor.SimWorld = newWorld.GetInternalAccessor().SimWorld;
            SimWorldAccessor.EntityManager = newWorld.GetInternalAccessor().EntityManager;
            SimWorldAccessor.SomeSimSystem = newWorld.GetInternalAccessor().SomeSimSystem; // could be any system

            HasJustRepacked = true;
            ReplaceVersion++;
            WorldReplaced?.Invoke();
        }

        public void RegisterIncomingEntityInjection(string sceneName)
        {
            _incomingEntityInjections.Add(sceneName);
        }

        public void UnregisterIncomingEntityInjection(string sceneName)
        {
            _incomingEntityInjections.Remove(sceneName);
        }

        private int _ongoingInjections = 0;
        private SimulationWorld _replaceWorld;
        private bool _inPlayerLoop;

        void IWorldOwner.OnBeginEntitiesInjectionFromGameObjectConversion(List<Scene> comingFromScenes)
        {
            foreach (var scene in comingFromScenes)
            {
                if (!_incomingEntityInjections.Contains(scene.name))
                {
                    Log.Error($"Unexpected entities coming from {scene.name} are being injected into the simulation. " +
                        $"This should not happen");
                }
            }

            if (_ongoingInjections == 0)
            {
                var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
                if (changeDetectionEnd != null)
                {
                    changeDetectionEnd.ForceEndSample();
                }
            }
            _ongoingInjections++;
        }

        void IWorldOwner.OnEndEntitiesInjectionFromGameObjectConversion()
        {
            _ongoingInjections--;

            if (_ongoingInjections == 0)
            {
                var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
                if (changeDetectionBegin != null)
                {
                    changeDetectionBegin.ResetSample();
                }
            }
        }

        public void ClearSimWorld()
        {
            if (SimulationWorld == null)
                return;

            // replace world with new empty world
            RequestReplaceSimWorld(CreateNewReplacementWorld());
            //var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
            //var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();

            //if (changeDetectionEnd != null)
            //{
            //    changeDetectionEnd.ForceEndSample();
            //}

            //World emptyWorld = new World("empty");

            //SimulationWorld.EntityManager.CopyAndReplaceEntitiesFrom(emptyWorld.EntityManager);

            //if (changeDetectionBegin != null)
            //{
            //    changeDetectionBegin.ResetSample();
            //}
            //Log.Info($"Clear done");
            //emptyWorld.Dispose();
        }

        public static void ClearAllSimulationWorlds()
        {
            List<World> worlds = new List<World>();
            foreach (World world in World.All) // copy world list to avoid 'Collection modified' exception
            {
                worlds.Add(world);
            }

            foreach (World world in worlds)
            {
                if (!world.IsCreated)
                    continue;

                var simWorldOwner = world.GetExistingSystem<SimulationWorldSystem>();
                if (simWorldOwner != null)
                {
                    simWorldOwner.ClearSimWorld();
                }
            }
        }

        private static void CreateSimSystems(World world)
        {
            var simPreInitGroup = world.CreateSystem<SimPreInitializationSystemGroup>();
            var simInitGroup = world.CreateSystem<SimInitializationSystemGroup>();
            var simSimGroup = world.CreateSystem<SimSimulationSystemGroup>();
            var simPresGroup = world.CreateSystem<SimPresentationSystemGroup>();
            var simPostPresGroup = world.CreateSystem<SimPostPresentationSystemGroup>();

            // pre init group (not visible in EntityDebugger for some reason ...)
#if SAFETY
            simPreInitGroup.AddSystemToUpdateList(world.CreateSystem<ChangeDetectionSystemEnd>());
#endif
            simPreInitGroup.AddSystemToUpdateList(world.CreateSystem<UpdateSimulationTimeSystem>());
            simPreInitGroup.AddSystemToUpdateList(world.CreateSystem<InitializeRandomSeedSystem>());

            // init group
            simInitGroup.AddSystemToUpdateList(world.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
            simInitGroup.AddSystemToUpdateList(world.CreateSystem<EndInitializationEntityCommandBufferSystem>());

            // sim group
            simSimGroup.AddSystemToUpdateList(world.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
            CCC.Fix2D.Fix2DHelpers.AddPhysicsSystemsToGroup(world, simSimGroup);
            simSimGroup.AddSystemToUpdateList(world.CreateSystem<EndSimulationEntityCommandBufferSystem>());

            // pres group
            simPresGroup.AddSystemToUpdateList(world.CreateSystem<BeginPresentationEntityCommandBufferSystem>());

            // post pres
#if SAFETY
            simPostPresGroup.AddSystemToUpdateList(world.CreateSystem<ChangeDetectionSystemBegin>());
#endif
            // Add Systems To Root Level System Groups
            {
                // create presentation system and simulation system
                var initializationSystemGroup = world.GetOrCreateSystem<SimInitializationSystemGroup>();
                var simulationSystemGroup = world.GetOrCreateSystem<SimSimulationSystemGroup>();
                var presentationSystemGroup = world.GetOrCreateSystem<SimPresentationSystemGroup>();

                // Add systems to their groups, based on the [UpdateInGroup] attribute.
                foreach (var type in AllSimSystemTypes)
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
                            Log.Error($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} must be derived from ComponentSystemGroup.");
                            continue;
                        }

                        if (type == typeof(SimPreInitializationSystemGroup) ||
                            type == typeof(SimPostPresentationSystemGroup))
                        {
                            Log.Error($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} is reserved for internal stuff.");
                            continue;
                        }

                        // Warn against unexpected behaviour combining DisableAutoCreation and UpdateInGroup
                        var parentDisableAutoCreation = Attribute.IsDefined(group.GroupType, typeof(DisableAutoCreationAttribute));
                        if (parentDisableAutoCreation)
                        {
                            Log.Warning($"A system {type} wants to execute in {group.GroupType} but this group has [DisableAutoCreation] and {type} does not.");
                        }

                        var groupSys = GetOrCreateSystemAndLogException(world, group.GroupType) as ComponentSystemGroup;
                        if (groupSys == null)
                        {
                            Log.Warning(
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

            ComponentSystemBase GetOrCreateSystemAndLogException(World world, Type type)
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
                    Log.Exception(e);
                    return null;
                }
            }
        }

    }


}