using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(TickSimulationSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class ReceiveSimulationTickSystem : ComponentSystem
    {
        public int SimTicksInQueue => _simTicksDropper.QueueLength;
        public float CurrentSimPlayingSpeed => _simTicksDropper.Speed;

        private SessionInterface _session;

        private TickSimulationSystem _tickSystem;

        private SelfRegulatingDropper<NetMessageSimTick> _simTicksDropper = new SelfRegulatingDropper<NetMessageSimTick>(
                maximalCatchUpSpeed: SimulationConstants.CLIENT_SIM_TICK_MAX_CATCH_UP_SPEED,
                maximalExpectedTimeInQueue: SimulationConstants.CLIENT_SIM_TICK_MAX_EXPECTED_TIME_IN_QUEUE);

        private bool _shelveTicks = false;

        // used while we are in sync process
        private List<NetMessageSimTick> _shelvedSimTicks = new List<NetMessageSimTick>();

        protected override void OnCreate()
        {
            base.OnCreate();

            _session = OnlineService.OnlineInterface?.SessionInterface;

            if (_session == null)
                throw new NullReferenceException();

            if (!_session.IsClientType)
                throw new Exception($"{nameof(ReceiveSimulationTickSystem)} expects the session interface to be of type 'Client'.");

            _session.RegisterNetMessageReceiver<NetMessageSimTick>(OnNetMessageSimTick);

            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnDestroy()
        {
            _session.UnregisterNetMessageReceiver<NetMessageSimTick>(OnNetMessageSimTick);

            base.OnDestroy();
        }

        private void OnNetMessageSimTick(NetMessageSimTick tick, INetworkInterfaceConnection serverConnection)
        {
            if (_shelveTicks)
            {
                // if we receive ticks while we're syncing, shelve the tick so we can restored it later
                _shelvedSimTicks.Add(tick);
                Debug.Log($"Receive Tick {tick.TickData.ExpectedNewTickId} (SHELVE)");
                return;
            }

            // The server has sent a tick message
            _simTicksDropper.Enqueue(tick, (float)SimulationConstants.TIME_STEP);
            Debug.Log($"Receive Tick {tick.TickData.ExpectedNewTickId}");
        }

        public void StartShelvingTicks()
        {
            Debug.Log($"START SHELVING");
            _shelveTicks = true;
        }

        public void StopShelvingTicks()
        {
            Debug.Log($"STOP SHELVING");
            _shelveTicks = false;
        }

        public void ClearAccumulatedTicks()
        {
            _simTicksDropper.Clear();
        }

        public void RestoreTicksFromShelf(uint minimumTickId)
        {
            for (int i = 0; i < _shelvedSimTicks.Count; i++)
            {
                if (_shelvedSimTicks[i].TickData.ExpectedNewTickId >= minimumTickId)
                {
                    _simTicksDropper.Enqueue(_shelvedSimTicks[i], (float)SimulationConstants.TIME_STEP);
                    Debug.Log($"Unshelve Tick {_shelvedSimTicks[i].TickData.ExpectedNewTickId}");
                }
            }
        }

        protected override void OnUpdate()
        {
            _simTicksDropper.Update(Time.DeltaTime);

            while (_tickSystem.CanTick && _simTicksDropper.TryDrop(out NetMessageSimTick tick))
            {
                _tickSystem.AvailableTicks.Add(tick.TickData);
            }
        }
    }

}