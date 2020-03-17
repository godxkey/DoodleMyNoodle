using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace SimulationControl
{
    [UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
    public class SimulationControlSystemGroup : ComponentSystemGroup
    {
        private SimulationWorldSystem _simulationWorldSystem;

        private List<ComponentSystemBase> _manuallyCreatedSystems = new List<ComponentSystemBase>();

        public void Initialize(SessionInterface sessionInterface, ConstructSimulationTickSystem.ValidationDelegate simInputValidationMethod)
        {
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

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _simulationWorldSystem.ClearSimWorld();
            _simulationWorldSystem.ReadyForEntityInjections = true;

            ManualCreateAndAddSystem<SubmitSimulationInputSystem>();
            ManualCreateAndAddSystem<LoadSimulationSceneSystem>();
            ManualCreateAndAddSystem<SaveAndLoadSimulationSystem>();

            switch (sessionInterface)
            {
                case SessionClientInterface _:
                    ManualCreateAndAddSystem<ReceiveSimulationSyncSystem>();
                    ManualCreateAndAddSystem<ReceiveSimulationTickSystem>();
                    break;

                case SessionServerInterface _:
                    ManualCreateAndAddSystem<SendSimulationSyncSystem>();
                    ManualCreateAndAddSystem<SendSimulationTickSystem>();
                    ManualCreateAndAddSystem<ReceiveSimulationInputSystem>();
                    ManualCreateAndAddSystem<ConstructSimulationTickSystem>().ValidationMethod = simInputValidationMethod;
                    break;

                case null: // Local play
                    ManualCreateAndAddSystem<ConstructSimulationTickSystem>().ValidationMethod = simInputValidationMethod;
                    break;
            }


            SortSystemUpdateList();
        }

        public void Shutdown()
        {
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