using CCC.Operations;
using System.Collections;
using Unity.Entities;

public class SimulationSyncFromTransferClientOperation : CoroutineOperation
{
    public bool IsServerReadyToSendSimTicksForSimulationWeAreSyncingTo { get; private set; }

    SessionClientInterface _sessionInterface;
    private World _simulationWorld;
    bool _receivedResponseFromServer;
    byte[] _simData;

    public SimulationSyncFromTransferClientOperation(SessionClientInterface sessionInterface, World simulationWorld)
    {
        _sessionInterface = sessionInterface;
        _simulationWorld = simulationWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // request sync to server
        NetMessageRequestSimSync syncRequest = new NetMessageRequestSimSync();
        _sessionInterface.SendNetMessageToServer(syncRequest);

        // wait for server response
        _sessionInterface.RegisterNetMessageReceiver<NetMessageSerializedSimulation>(OnNetMessageSerializedSimulation);

        // wait for _loadPath to be assigned
        while (_receivedResponseFromServer == false)
        {
            yield return null;
        }

        _sessionInterface.UnregisterNetMessageReceiver<NetMessageSerializedSimulation>(OnNetMessageSerializedSimulation);

        // terminate if load path is invalid
        if (_simData == null || _simData.Length == 0)
        {
            TerminateWithAbnormalFailure($"Invalid simulation data received from the server. Prehaps it failed to serialize it.");
            yield break;
        }

        yield return ExecuteSubOperationAndWaitForSuccess(new Sim.Operations.SimDeserializationOperation(_simData, _simulationWorld));

        TerminateWithSuccess();
    }

    private void OnNetMessageSerializedSimulation(NetMessageSerializedSimulation netMessage, INetworkInterfaceConnection source)
    {
        _simData = netMessage.SerializedSimulation;
        _receivedResponseFromServer = true;
        IsServerReadyToSendSimTicksForSimulationWeAreSyncingTo = true;
    }

    protected override void OnTerminate()
    {
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageSerializedSimulation>(OnNetMessageSerializedSimulation);
    }
}