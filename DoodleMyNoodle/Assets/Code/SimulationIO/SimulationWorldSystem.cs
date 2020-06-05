using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace SimulationControl
{
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SimulationWorldSystem : ComponentSystem, IWorldOwner
    {
        List<string> _incomingEntityInjections = new List<string>();

        World IWorldOwner.OwnedWorld => SimulationWorld;
        internal SimulationWorld SimulationWorld { get; private set; }
        public World PresentationWorld => World;
        public ExternalSimWorldAccessor SimWorldAccessor { get; private set; }
        public bool ReadyForEntityInjections { get; set; } = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            SimulationWorld = new SimulationWorld("Simulation World");
            SimulationWorld.Owner = this;

            World.GetOrCreateSystem<LoadSimulationSceneSystem>();
            World.GetOrCreateSystem<TickSimulationSystem>();

            SimWorldAccessor = new ExternalSimWorldAccessor();
            SimWorldAccessor.SimWorld = SimulationWorld.GetInternalAccessor().SimWorld;
            SimWorldAccessor.EntityManager = SimulationWorld.GetInternalAccessor().EntityManager;
            SimWorldAccessor.SomeSimSystem = SimulationWorld.GetInternalAccessor().SomeSimSystem; // could be any system
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (SimulationWorld.IsCreated)
                SimulationWorld.Dispose();

            SimulationWorld = null;
        }

        protected override void OnUpdate() { }


        public void RegisterIncomingEntityInjection(string sceneName)
        {
            _incomingEntityInjections.Add(sceneName);
        }

        public void UnregisterIncomingEntityInjection(string sceneName)
        {
            _incomingEntityInjections.Remove(sceneName);
        }

        private int _ongoingInjections = 0;

        void IWorldOwner.OnBeginEntitiesInjectionFromGameObjectConversion(List<Scene> comingFromScenes)
        {
            DebugService.Log($"OnBeginEntitiesInjectionFromGameObjectConversion");
            foreach (var scene in comingFromScenes)
            {
                if (!_incomingEntityInjections.Contains(scene.name))
                {
                    DebugService.LogError($"Unexpected entities coming from {scene.name} are being injected into the simulation. " +
                        $"This should not happen");
                }
            }

            if (_ongoingInjections == 0)
            {
                var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
                if (changeDetectionEnd != null)
                {
                    DebugService.Log($"OnBeginEntitiesInjectionFromGameObjectConversion.ForceEndSample");
                    changeDetectionEnd.ForceEndSample();
                }
            }
            _ongoingInjections++;
        }

        void IWorldOwner.OnEndEntitiesInjectionFromGameObjectConversion()
        {
            DebugService.Log($"OnEndEntitiesInjectionFromGameObjectConversion");
            _ongoingInjections--;

            if (_ongoingInjections == 0)
            {
                var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
                if (changeDetectionBegin != null)
                {
                    DebugService.Log($"OnEndEntitiesInjectionFromGameObjectConversion.ResetSample");
                    changeDetectionBegin.ResetSample();
                }
            }
        }

        public void ClearSimWorld()
        {
            DebugService.Log($"Clearing {SimulationWorld.Name} ...");
            var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
            var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();

            if (changeDetectionEnd != null)
            {
                changeDetectionEnd.ForceEndSample();
            }

            SimulationWorld.EntityManager.DestroyEntity(SimulationWorld.EntityManager.UniversalQuery);

            if (changeDetectionBegin != null)
            {
                changeDetectionBegin.ResetSample();
            }
        }

        //private void SetChangeDetectionLogMode(ChangeDetectionSystemEnd.LogMode logMode)
        //{
        //    var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
        //    if (changeDetectionEnd != null)
        //    {
        //        changeDetectionEnd.LoggingMode = logMode;
        //    }
        //}

        public static void ClearAllSimulationWorlds()
        {
            foreach (World world in World.All)
            {
                var simWorldOwner = world.GetExistingSystem<SimulationWorldSystem>();
                if (simWorldOwner != null)
                {
                    simWorldOwner.ClearSimWorld();
                }
            }
        }
    }


}