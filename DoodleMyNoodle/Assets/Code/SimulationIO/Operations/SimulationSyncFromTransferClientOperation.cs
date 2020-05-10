using CCC.Operations;
using System;
using System.Collections;
using Unity.Entities;

public class AwaitNetMessage<T> : IDisposable
{
    private SessionInterface _sessionInterface;
    private bool _hasReceivedResponse;

    public T Response;
    public INetworkInterfaceConnection Source;

    public AwaitNetMessage(SessionInterface sessionInterface)
    {
        _hasReceivedResponse = false;
        _sessionInterface = sessionInterface ?? throw new ArgumentNullException(nameof(sessionInterface));
    }

    public IEnumerator WaitForResponse()
    {
        _sessionInterface.RegisterNetMessageReceiver<T>(OnResponse);

        while (_hasReceivedResponse)
        {
            yield return null;
        }

        _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
    }

    private void OnResponse(T arg1, INetworkInterfaceConnection arg2)
    {
        _hasReceivedResponse = true;
        Response = arg1;
        Source = arg2;
    }

    public void Dispose()
    {
        _sessionInterface.UnregisterNetMessageReceiver<T>(OnResponse);
    }
}

//public class SimulationSyncFromTransferOrDiskClientOperation : CoroutineOperation
//{
//    public bool IsServerReadyToSendSimTicksForSimulationWeAreSyncingTo { get; private set; }

//    SessionClientInterface _sessionInterface;
//    private World _simulationWorld;
//    bool _receivedResponseFromServer;
//    string _simData;

//    public SimulationSyncFromTransferOrDiskClientOperation(SessionClientInterface sessionInterface, World simulationWorld)
//    {
//        _sessionInterface = sessionInterface;
//        _simulationWorld = simulationWorld;
//    }

//    protected override IEnumerator ExecuteRoutine()
//    {
//        // request sync to server
//        _sessionInterface.SendNetMessageToServer(new NetMessageRequestSimSync()
//        {
//            AttemptTransferByDisk = true,
//            LocalMachineName = Environment.MachineName
//        });

//        var await = DisposeOnTerminate(new AwaitNetMessage<NetMessageAcceptSimSync>(_sessionInterface));

//        yield return await.WaitForResponse();

//        await.Response.
//    }
//}

public class SimulationSyncFromTransferClientOperation : CoroutineOperation
{
    public bool IsServerReadyToSendSimTicksForSimulationWeAreSyncingTo { get; private set; }

    SessionClientInterface _sessionInterface;
    private World _simulationWorld;
    bool _receivedResponseFromServer;
    string _simData;

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

        //_sessionInterface.RegisterNetMessageReceiver<NetMessageAcceptSimSync>(OnNetMessageAcceptSync);

        ////todo

        //_sessionInterface.UnregisterNetMessageReceiver<NetMessageAcceptSimSync>(OnNetMessageAcceptSync);

        // wait for server response
        _sessionInterface.RegisterNetMessageReceiver<NetMessageSerializedSimulation>(OnNetMessageSerializedSimulation);

        // wait for _loadPath to be assigned
        while (_receivedResponseFromServer == false)
        {
            yield return null;
        }

        _sessionInterface.UnregisterNetMessageReceiver<NetMessageSerializedSimulation>(OnNetMessageSerializedSimulation);

        // terminate if load path is invalid
        if (_simData.IsNullOrEmpty())
        {
            TerminateWithFailure($"Invalid simulation data received from the server. Prehaps it failed to serialize it.");
            yield break;
        }

        yield return ExecuteSubOperationAndWaitForSuccess(SimulationView.DeserializeSimulation(_simData, _simulationWorld));

        TerminateWithSuccess();
    }

    private void OnNetMessageAcceptSync(NetMessageAcceptSimSync arg1, INetworkInterfaceConnection arg2)
    {
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