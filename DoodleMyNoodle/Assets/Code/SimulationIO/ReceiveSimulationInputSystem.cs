using System;
using Unity.Entities;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(ConstructSimulationTickSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class ReceiveSimulationInputSystem : ComponentSystem
    {
        private SessionInterface _session;
        private ConstructSimulationTickSystem _constructTickSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _session = OnlineService.OnlineInterface?.SessionInterface;

            _session.RegisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);
            _constructTickSystem = World.GetOrCreateSystem<ConstructSimulationTickSystem>();
        }

        protected override void OnDestroy()
        {
            _session.UnregisterNetMessageReceiver<NetMessageInputSubmission>(OnNetMessageInputSubmission);

            base.OnDestroy();
        }

        private void OnNetMessageInputSubmission(NetMessageInputSubmission netMessage, INetworkInterfaceConnection source)
        {
            // A client wants to submit a new message
            _constructTickSystem.SubmitInputInternal(netMessage.input, source, netMessage.submissionId);
        }

        protected override void OnUpdate() { }
    }
}