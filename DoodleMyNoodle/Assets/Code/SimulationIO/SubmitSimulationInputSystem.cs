using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngineX;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SubmitSimulationInputSystem : ComponentSystem
    {
        private SessionInterface GetSession() => OnlineService.OnlineInterface?.SessionInterface;

        private SimulationWorldSystem _simWorldSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _simWorldSystem.SimWorldAccessor.SubmitSystem = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_simWorldSystem?.SimWorldAccessor != null)
                _simWorldSystem.SimWorldAccessor.SubmitSystem = null;
        }

        protected override void OnUpdate() { }

        public InputSubmissionId SubmitInput(SimInput input)
        {
            if (input == null)
            {
                Log.Error("Trying to submit a null input");
                return InputSubmissionId.Invalid;
            }

            var session = GetSession();
            if (session != null && session is SessionClientInterface clientSession)
            {
                // CLIENT
                var syncSystem = World.GetExistingSystem<ReceiveSimulationSyncSystem>();
                if (syncSystem != null && syncSystem.IsSynchronizing)
                {
                    Log.Info("Discarding input since we are syncing to the simulation");
                    return InputSubmissionId.Invalid;
                }

                var submissionId = InputSubmissionId.Generate();
                clientSession.SendNetMessageToServer(new NetMessageInputSubmission()
                {
                    submissionId = submissionId,
                    input = input
                });
                return submissionId;
            }
            else
            {
                // SERVER AND LOCAL
                var submissionId = InputSubmissionId.Generate();
                World.GetOrCreateSystem<ConstructSimulationTickSystem>()
                    .SubmitInputInternal(
                    input: input,
                    instigatorConnection: null, // local connection => null
                    submissionId: submissionId);
                return submissionId;
            }
        }
    }
}