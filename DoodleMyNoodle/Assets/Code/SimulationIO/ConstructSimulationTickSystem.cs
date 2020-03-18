using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

namespace SimulationControl
{
    /// <summary>
    /// This system constructs simulation ticks based on the received inputs
    /// </summary>
    [DisableAutoCreation]
    [UpdateBefore(typeof(TickSimulationSystem))]
    [UpdateAfter(typeof(SubmitSimulationInputSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class ConstructSimulationTickSystem : ComponentSystem
    {
        [ConfigVar("sim.tick_rate_scale", "1", description: "The number of ticks executed per fixed update")]
        static ConfigVar s_tickRateScale;

        private float _tickTimeCounter;
        private Queue<SimInputSubmission> _inputSubmissionQueue = new Queue<SimInputSubmission>();
        private TickSimulationSystem _tickSystem;
        private SimulationWorldSystem _simWorldSystem;
        private SendSimulationTickSystem _sendTickSystem;

        public delegate bool ValidationDelegate(SimInput input, INetworkInterfaceConnection instigator);

        public ValidationDelegate ValidationMethod;

        protected override void OnCreate()
        {
            base.OnCreate();

            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _sendTickSystem = World.GetExistingSystem<SendSimulationTickSystem>();
        }

        //public bool IsLocallySubmittedInputInQueue(InputSubmissionId inputSubmissionId)
        //{
        //    foreach (var item in _inputSubmissionQueue)
        //    {
        //        if (item.InstigatorConnectionId == uint.MaxValue && item.ClientSubmissionId == inputSubmissionId)
        //            return true;
        //    }

        //    return false;
        //}

        public void SubmitInputInternal(SimInput input, INetworkInterfaceConnection instigatorConnection, InputSubmissionId submissionId)
        {
            if (ValidateInput(input, instigatorConnection))
            {
                _inputSubmissionQueue.Enqueue(new SimInputSubmission()
                {
                    Input = input,
                    InstigatorConnectionId = instigatorConnection == null ? uint.MaxValue : instigatorConnection.Id,
                    ClientSubmissionId = submissionId
                });
            }
        }

        private bool ValidateInput(SimInput input, INetworkInterfaceConnection instigatorConnection)
        {
            // we don't accept any input while sim is paused
            if (!_tickSystem.CanTick)
                return false;

            if (input is SimMasterInput && instigatorConnection != null)
            {
                DebugService.LogWarning($"Connection({instigatorConnection.Id}) tried to submit a Master input {input.GetType()}." +
                    $" Only the local master of the simulation is allowed to submit those.");
                return false;
            }

            if (ValidationMethod != null)
            {
                return ValidationMethod(input, instigatorConnection);
            }
            else
            { 
                return true;
            }
        }

        protected override void OnUpdate()
        {
            // this counter makes sure we construct ticks at the desired rate
            _tickTimeCounter += Time.DeltaTime * s_tickRateScale.FloatValue;

            while (_tickTimeCounter > SimulationConstants.TIME_STEP_F)
            {
                if (_tickSystem.CanTick)
                {
                    // bundle them up in a tick
                    SimTickData tickData = new SimTickData()
                    {
                        InputSubmissions = _inputSubmissionQueue.ToArray(),
                        ExpectedNewTickId = FindExpectedNewTickId()
                    };

                    _sendTickSystem?.ConstructedTicks.Add(tickData);

                    // give tick to tick system    fbessette: Maybe we should go through entities to communicate that data ?
                    _tickSystem.AvailableTicks.Add(tickData);
                }

                _inputSubmissionQueue.Clear();

                _tickTimeCounter -= SimulationConstants.TIME_STEP_F;
            }
        }

        uint FindExpectedNewTickId()
        {
            if (_tickSystem.AvailableTicks.Count > 0)
            {
                return _tickSystem.AvailableTicks.Last().ExpectedNewTickId + 1;
            }
            else
            {
                if (_simWorldSystem.SimWorldAccessor.HasSingleton<SimulationOngoingTickId>())
                {
                    return _simWorldSystem.SimWorldAccessor.GetSingleton<SimulationOngoingTickId>().TickId + 1;
                }
                else
                {
                    return default(uint) + 1;
                }
            }
        }
    }

}