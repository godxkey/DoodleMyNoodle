using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

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
        [ConsoleVar("Sim.TickSpeed", "The speed at which sim ticks are executed. 1 = default. 2 = twice as fast")]
        static float s_tickSpeed = 1;

        private float _tickTimeCounter;
        private Queue<SimInputSubmission> _inputSubmissionQueue = new Queue<SimInputSubmission>();
        private TickSimulationSystem _tickSystem;
        private SimulationWorldSystem _simWorldSystem;

        public delegate bool ValidationDelegate(SimInput input, INetworkInterfaceConnection instigator);

        public ValidationDelegate ValidationMethod;

        protected override void OnCreate()
        {
            base.OnCreate();

            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
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
                Log.Info(SimulationIO.LogChannel, $"Accepted sim input from '{(instigatorConnection == null ? "local player" : instigatorConnection.Id.ToString())}': {input}");

                _inputSubmissionQueue.Enqueue(new SimInputSubmission()
                {
                    Input = input,
                    InstigatorConnectionId = instigatorConnection == null ? uint.MaxValue : instigatorConnection.Id,
                    ClientSubmissionId = submissionId
                });
            }
            else
            {
                Log.Info(SimulationIO.LogChannel, $"Refused sim input from '{(instigatorConnection == null ? "local player" : instigatorConnection.Id.ToString())}': {input}");
            }
        }

        private bool ValidateInput(SimInput input, INetworkInterfaceConnection instigatorConnection)
        {
            // we don't accept any input while sim is paused
            if (!_tickSystem.CanTick)
                return false;

            if (input is SimMasterInput && instigatorConnection != null)
            {
                Log.Warning($"Connection({instigatorConnection.Id}) tried to submit a Master input {input.GetType()}." +
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
            _tickTimeCounter += Time.DeltaTime * s_tickSpeed;

            while (_tickTimeCounter > SimulationConstants.TIME_STEP_F)
            {
                if (_tickSystem.CanTick)
                {
                    // bundle them up in a tick
                    SimTickData tickData = new SimTickData()
                    {
                        InputSubmissions = _inputSubmissionQueue.ToList(),
                        ExpectedNewTickId = FindExpectedNewTickId()
                    };
                    _inputSubmissionQueue.Clear();

                    var sendTickSystem = World.GetExistingSystem<SendSimulationTickSystem>();

                    if (sendTickSystem != null)
                    {
                        while (sendTickSystem.ExceedsSizeLimit(tickData))
                        {
                            _inputSubmissionQueue.Enqueue(tickData.InputSubmissions.Pop());
                        }

                        if (_inputSubmissionQueue.Count > 100)
                        {
                            Log.Warning("Too many sim inputs clugging the tick creation. Clearing the list...");
                            _inputSubmissionQueue.Clear();
                        }
                    }

                    sendTickSystem?.ConstructedTicks.Add(tickData);

                    // give tick to tick system
                    _tickSystem.AvailableTicks.Add(tickData);

                    Log.Info(SimulationIO.LogChannel, $"Construct tick '{tickData.ExpectedNewTickId}' with {tickData.InputSubmissions.Count} inputs. " +
                        $"{(_inputSubmissionQueue.Count > 0 ? $"{_inputSubmissionQueue.Count} input(s) were buffered onto the next tick because of size limit." : "")}");
                }


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