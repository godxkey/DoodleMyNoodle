using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.LowLevel;

namespace SimulationControl
{
    [UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class SimulationControlSystemGroup : ManualCreationComponentSystemGroup
    {
        private SimulationWorldSystem _simulationWorldSystem;

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
            World.GetOrCreateSystem<ViewSystemGroup>().Initialize(this);

            ManualCreateAndAddSystem<SubmitSimulationInputSystem>();
            ManualCreateAndAddSystem<LoadSimulationSceneSystem>();
            ManualCreateAndAddSystem<SaveAndLoadSimulationSystem>();
            ManualCreateAndAddSystem<ChecksumSystem>();

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
                ManualCreateAndAddSystem<RequestChecksumSystem>(); // todo: move back to server


#if UNITY_EDITOR
            // This is a hack to force the EntityDebugger to correctly update the list of displayed systems
            {
                PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopSystem[] oldArray = playerLoop.subSystemList;
                playerLoop.subSystemList = new PlayerLoopSystem[oldArray.Length];
                Array.Copy(oldArray, 0, playerLoop.subSystemList, 0, oldArray.Length);
                PlayerLoop.SetPlayerLoop(playerLoop);
            }
#endif
        }

        public void Shutdown()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;

            World.GetExistingSystem<ViewSystemGroup>()?.Shutdown();

            if (_simulationWorldSystem.SimulationWorld != null && _simulationWorldSystem.SimulationWorld.IsCreated)
            {
                _simulationWorldSystem.ReadyForEntityInjections = false;
                _simulationWorldSystem.ClearSimWorld();
            }

            DestroyAllManuallyCreatedSystems();
        }
    }

}