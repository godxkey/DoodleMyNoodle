using CCC.Operations;
using Sim.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSyncFromTransferServerOperation : CoroutineOperation
{
    INetworkInterfaceConnection _client;
    SessionServerInterface _sessionInterface;

    public SimulationSyncFromTransferServerOperation(SessionServerInterface sessionInterface, INetworkInterfaceConnection clientConnection)
    {
        _client = clientConnection;
        _sessionInterface = sessionInterface;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        uint tickId = SimulationView.TickId;
        
        // Serialize sim
        SimSerializationOperationWithCache serializeOp = SimulationView.SerializeSimulation();
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);
        
        string serializedData = serializeOp.SerializationData;


        // Transfer sim
        NetMessageSerializedSimulation netMessage = new NetMessageSerializedSimulation()
        {
            SerializedSimulation = serializedData
        };
        var transferOp = _sessionInterface.BeginLargeDataTransfer(netMessage, _client, description: $"Simulation-{tickId}");
        yield return ExecuteSubOperationAndWaitForSuccess(transferOp);
        
        // Terminate
        TerminateWithSuccess($"Simulation sent to client.");
    }
}