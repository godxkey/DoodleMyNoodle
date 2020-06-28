using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngineX;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    [UpdateBefore(typeof(TickSimulationSystem))]
    public class SendSimulationSyncSystem : ComponentSystem
    {
        [ConsoleVar("Sim.PauseWhileOthersJoin", "Should the simulation be paused while players are joining the game?", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
        static bool s_pauseSimulationWhilePlayersAreJoining = true;

        private SessionServerInterface _session;
        private List<CoroutineOperation> _ongoingOperations = new List<CoroutineOperation>();
        private SimulationWorldSystem _simWorldSystem;
        private TickSimulationSystem _tickSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _session = OnlineService.ServerInterface.SessionServerInterface;
            _session.RegisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);

            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnDestroy()
        {
            _session.UnregisterNetMessageReceiver<NetMessageRequestSimSync>(OnSimSyncRequest);

            foreach (var item in _ongoingOperations)
            {
                if (item.IsRunning)
                    item.TerminateWithAbnormalFailure();
            }

            base.OnDestroy();
        }

        private void OnSimSyncRequest(NetMessageRequestSimSync requestSync, INetworkInterfaceConnection source)
        {
            LaunchSyncForClient(source);
        }

        protected override void OnUpdate()
        {
            if(_ongoingOperations.Count > 0)
            {
                for (int i = _ongoingOperations.Count - 1; i >= 0; i--)
                {
                    if (!_ongoingOperations[i].IsRunning)
                        _ongoingOperations.RemoveAt(i);
                }

                if(_ongoingOperations.Count == 0)
                {
                    _tickSystem.UnpauseSimulation(key: "PlayerJoining");
                }
            }
        }


        SimulationSyncFromTransferServerOperation LaunchSyncForClient(INetworkInterfaceConnection clientConnection)
        {
            Log.Info($"Starting new sync...");

            var newOp = new SimulationSyncFromTransferServerOperation(_session, clientConnection, _simWorldSystem.SimulationWorld);

            newOp.OnFailCallback = (op) =>
            {
                Log.Info($"Sync failed. {op.Message}");
            };

            newOp.OnSucceedCallback = (op) =>
            {
                Log.Info($"Sync complete. {op.Message}");
            };

            newOp.Execute();

            _ongoingOperations.Add(newOp);

            if (s_pauseSimulationWhilePlayersAreJoining)
            {
                _tickSystem.PauseSimulation(key: "PlayerJoining");
            }

            return newOp;
        }
    }
}