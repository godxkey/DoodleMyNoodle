using System;
using System.Collections.Generic;
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

        protected override void OnCreate()
        {
            base.OnCreate();

            SimWorldAccessor = new ExternalSimWorldAccessor();
            World.GetOrCreateSystem<LoadSimulationSceneSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();

            RequestReplaceSimWorld(CreateNewReplacementWorld());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (SimulationWorld.IsCreated)
                SimulationWorld.Dispose();

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
            return new SimulationWorld($"Simulation World (replace ver:{ReplaceVersion})");
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

            SimulationWorld = newWorld;

            newWorld.Owner = this;

            _tickSystem.CreateSimSystemsAndAddToPlayerLoop(newWorld);
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
    }


}