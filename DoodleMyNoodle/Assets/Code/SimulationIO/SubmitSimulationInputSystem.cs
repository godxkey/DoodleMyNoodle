using System.Collections;
using System.Collections.Generic;
using Unity.Entities;


namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SubmitSimulationInputSystem : ComponentSystem
    {
        private SessionInterface GetSession() => OnlineService.OnlineInterface?.SessionInterface;

        protected override void OnUpdate() { }

        public void SubmitInput(SimInput input)
        {
            if (input == null)
            {
                DebugService.LogError("Trying to submit a null input");
                return;
            }

            var session = GetSession();
            if (session != null && session is SessionClientInterface clientSession)
            {
                // CLIENT
                var syncSystem = World.GetExistingSystem<ReceiveSimulationSyncSystem>();
                if (syncSystem != null && syncSystem.IsSynchronizing)
                {
                    DebugService.Log("Discarding input since we are syncing to the simulation");
                    return;
                }

                clientSession.SendNetMessageToServer(new NetMessageInputSubmission()
                {
                    submissionId = InputSubmissionId.Generate(),
                    input = input
                });
            }
            else
            {
                // SERVER AND LOCAL
                World.GetOrCreateSystem<ConstructSimulationTickSystem>()
                    .SubmitInputInternal(
                    input: input,
                    instigatorConnection: null, // local connection => null
                    submissionId: default);
            }
        }
    }
}