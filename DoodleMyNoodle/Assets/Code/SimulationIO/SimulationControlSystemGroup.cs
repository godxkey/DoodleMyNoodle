using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

namespace SimulationControl
{
    [UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class SimulationControlSystemGroup : ComponentSystemGroup
    {
        private SimulationWorldSystem _simulationWorldSystem;

        private List<ComponentSystemBase> _manuallyCreatedSystems = new List<ComponentSystemBase>();

        public bool IsMaster => IsLocal || IsServer;
        public bool IsClient { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsLocal { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize(SessionInterface sessionInterface, ConstructSimulationTickSystem.ValidationDelegate simInputValidationMethod)
        {
            if (IsInitialized)
                throw new Exception($"{nameof(SimulationControlSystemGroup)} is already initialized");

            IsInitialized = true;

            T ManualCreateAndAddSystem<T>() where T : ComponentSystem
            {
                if (!Attribute.IsDefined(typeof(T), typeof(DisableAutoCreationAttribute)))
                {
                    DebugService.LogError("We should not be manually creating the system here since its going to create itself anyway");
                }

                var sys = World.GetOrCreateSystem<T>();
                _manuallyCreatedSystems.Add(sys);
                AddSystemToUpdateList(sys);
                return sys;
            }

            switch (sessionInterface)
            {
                case SessionClientInterface _: // Client
                    IsClient = true;
                    IsLocal = false;
                    IsServer = false;
                    break;

                case null: // Local play
                    IsClient = false;
                    IsLocal = true;
                    IsServer = false;
                    break;

                case SessionServerInterface _:  // Server
                    IsClient = false;
                    IsLocal = false;
                    IsServer = true;
                    break;
            }

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _simulationWorldSystem.ClearSimWorld();
            _simulationWorldSystem.ReadyForEntityInjections = true;

            ManualCreateAndAddSystem<SubmitSimulationInputSystem>();
            ManualCreateAndAddSystem<LoadSimulationSceneSystem>();
            ManualCreateAndAddSystem<SaveAndLoadSimulationSystem>();


            if (IsMaster)
            {
                ManualCreateAndAddSystem<ConstructSimulationTickSystem>().ValidationMethod = simInputValidationMethod;
            }

            if (IsClient)
            {
                ManualCreateAndAddSystem<ReceiveSimulationSyncSystem>();
                ManualCreateAndAddSystem<ReceiveSimulationTickSystem>();
            }

            if (IsServer)
            {
                ManualCreateAndAddSystem<SendSimulationSyncSystem>();
                ManualCreateAndAddSystem<SendSimulationTickSystem>();
                ManualCreateAndAddSystem<ReceiveSimulationInputSystem>();
            }


            SortSystemUpdateList();
        }

        public void Shutdown()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            if (_simulationWorldSystem.SimulationWorld != null && _simulationWorldSystem.SimulationWorld.IsCreated)
            {
                _simulationWorldSystem.ReadyForEntityInjections = false;
                _simulationWorldSystem.ClearSimWorld();
            }

            foreach (var sys in _manuallyCreatedSystems)
            {
                RemoveSystemFromUpdateList(sys);
                World.DestroySystem(sys);
            }
            _manuallyCreatedSystems.Clear();
        }
    }

}