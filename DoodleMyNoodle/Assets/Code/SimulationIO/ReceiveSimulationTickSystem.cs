using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(TickSimulationSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class ReceiveSimulationTickSystem : ComponentSystem
    {
        public int SimTicksInQueue => _simTicksDropper.QueueLength;
        public float CurrentSimPlayingSpeed => _simTicksDropper.Speed;
        public float SimTickQueueMaxDuration => _simTicksDropper.MaximalExpectedTimeInQueue;

        private SessionInterface _session;

        private TickSimulationSystem _tickSystem;
        private SelfRegulatingDropper<NetMessageSimTick> _simTicksDropper = new SelfRegulatingDropper<NetMessageSimTick>(
                maximalCatchUpSpeed: SimulationConstants.CLIENT_SIM_TICK_MAX_CATCH_UP_SPEED,
                maximalExpectedTimeInQueue: SimulationConstants.CLIENT_SIM_TICK_QUEUE_DURATION_MIN);

        private bool _shelveTicks = false;
        private List<NetMessageSimTick> _shelvedSimTicks = new List<NetMessageSimTick>(); // used while we are in sync process

        private double _lastTickReceiveTime;
        private CircularBuffer<double> _tickReceiveIntervals = new CircularBuffer<double>(SimulationConstants.CLIENT_SIM_TICK_QUEUE_DURATION_SAMPLE_SIZE);

        //private SpaceTimeDebugger.Clock _debugClock;
        //private SpaceTimeDebugger.Stream _debugStreamQueueLength;
        //private SpaceTimeDebugger.Stream _debugStreamTicksPerFrame;
        //private SpaceTimeDebugger.Stream _debugStreamMaxQueueDuration;

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

            //_debugStreamQueueLength = SpaceTimeDebugger.CreateStream("Sim Tick Queue", Color.yellow);
            //_debugStreamTicksPerFrame = SpaceTimeDebugger.CreateStream("Sim Ticks Per Frame", Color.magenta);
            //_debugStreamMaxQueueDuration = SpaceTimeDebugger.CreateStream("Sim Tick Queue Max Duration", Color.cyan);
        }

        protected override void OnDestroy()
        {
            _session.UnregisterNetMessageReceiver<NetMessageSimTick>(OnNetMessageSimTick);
            //_debugStreamQueueLength.Dispose();
            //_debugStreamTicksPerFrame.Dispose();
            //_debugStreamMaxQueueDuration.Dispose();
            base.OnDestroy();
        }

        private void OnNetMessageSimTick(NetMessageSimTick tick, INetworkInterfaceConnection serverConnection)
        {
            if (_shelveTicks)
            {
                // if we receive ticks while we're syncing, shelve the tick so we can restored it later
                Log.Info(SimulationIO.LogChannel, $"Receiving tick {tick.TickData.ExpectedNewTickId}. Shelving for later!");
                _shelvedSimTicks.Add(tick);
            }
            else
            {
                // The server has sent a tick message
                Log.Info(SimulationIO.LogChannel, $"Receiving tick {tick.TickData.ExpectedNewTickId}. Queueing for execution.");
                _simTicksDropper.Enqueue(tick, (float)SimulationConstants.TIME_STEP);

                double time = Time.ElapsedTime;
                double receiveInterval = Math.Min(time - _lastTickReceiveTime, SimulationConstants.CLIENT_SIM_TICK_QUEUE_DURATION_MAX_CONSIDERED_INTERVAL);

                _tickReceiveIntervals.PushBack(receiveInterval);
                _lastTickReceiveTime = time;
            }
        }

        public void StartShelvingTicks()
        {
            _shelveTicks = true;
        }

        public void StopShelvingTicks()
        {
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
                }
            }
        }

        protected override void OnUpdate()
        {
            double maxInterval = GetMaxRecentTickInterval();
            _simTicksDropper.MaximalExpectedTimeInQueue = (float)(maxInterval - (1f / SimulationConstants.TICK_RATE_F));

            //_debugStreamMaxQueueDuration.Log(_simTicksDropper.MaximalExpectedTimeInQueue);
            //_debugStreamQueueLength.Log(_simTicksDropper.QueueLength);

            _simTicksDropper.Update(Time.DeltaTime);

            int ticksThisFrame = 0;

            while (_tickSystem.CanTick && _simTicksDropper.TryDrop(out NetMessageSimTick tick))
            {
                ticksThisFrame++;
                _tickSystem.AvailableTicks.Add(tick.TickData);
            }

            //_debugStreamTicksPerFrame.Log(ticksThisFrame);
        }

        private double GetMaxRecentTickInterval()
        {
            double max = 0;
            foreach (var interval in _tickReceiveIntervals)
            {
                max = Math.Max(max, interval);
            }
            return max;
        }
    }

}