using CCC.Operations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    [UpdateBefore(typeof(ReceiveSimulationTickSystem))]
    public class ReceiveSimulationSyncSystem : ComponentSystem
    {
        private SessionClientInterface _session;
        private TickSimulationSystem _tickSystem;
        private SimulationWorldSystem _simWorldSystem;
        private ReceiveSimulationTickSystem _receiveTickSystem;

        private CoroutineOperation _ongoingSyncOp;

        public bool IsSynchronizing { get; private set; }

        private bool _requestSync = false;

        protected override void OnCreate()
        {
            base.OnCreate();

            _session = OnlineService.ClientInterface.SessionClientInterface;

            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _receiveTickSystem = World.GetOrCreateSystem<ReceiveSimulationTickSystem>();

            _requestSync = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_ongoingSyncOp != null && _ongoingSyncOp.IsRunning)
                _ongoingSyncOp.TerminateWithAbnormalFailure();
        }

        protected override void OnUpdate()
        {
            if (_requestSync)
            {
                SyncSimulationWithServer();
                _requestSync = false;
            }
        }

        CoroutineOperation SyncSimulationWithServer()
        {
            if (IsSynchronizing)
            {
                DebugService.LogWarning("Trying to start a SimSync process while we are already in one");
                return null;
            }

            _tickSystem.PauseSimulation(key: "sync");

            _receiveTickSystem.ClearAccumulatedTicks();
            _receiveTickSystem.StartShelvingTicks();

            _ongoingSyncOp = new SimulationSyncFromTransferClientOperation(_session, _simWorldSystem.SimulationWorld);

            _ongoingSyncOp.OnTerminateCallback = (op) =>
            {
                // restore ticks we received while syncing
                _receiveTickSystem.ClearAccumulatedTicks();
                _receiveTickSystem.RestoreTicksFromShelf(_simWorldSystem.SimulationWorld.GetLastedTickIdFromEntity() + 1);
                _receiveTickSystem.StopShelvingTicks();

                _tickSystem.UnpauseSimulation(key: "sync");
            };

            _ongoingSyncOp.OnSucceedCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Synced sim. {op.Message}");
            };

            _ongoingSyncOp.OnFailCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Failed to sync sim. {op.Message}");
            };

            _ongoingSyncOp.Execute();

            return _ongoingSyncOp;
        }
    }
}