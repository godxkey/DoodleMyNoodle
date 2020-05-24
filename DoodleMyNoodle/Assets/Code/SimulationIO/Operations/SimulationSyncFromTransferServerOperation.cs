using CCC.Operations;
using Sim.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimulationSyncFromTransferServerOperation : CoroutineOperation
{
    INetworkInterfaceConnection _client;
    SessionServerInterface _sessionInterface;
    private World _simulationWorld;

    public SimulationSyncFromTransferServerOperation(SessionServerInterface sessionInterface, INetworkInterfaceConnection clientConnection, World simulationWorld)
    {
        _client = clientConnection;
        _sessionInterface = sessionInterface;
        _simulationWorld = simulationWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // Serialize sim
        SimSerializationOperationWithCache serializeOp = SimulationView.SerializeSimulation(_simulationWorld);
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);

        string serializedData = serializeOp.SerializationData;


        // Transfer sim
        NetMessageSerializedSimulation netMessage = new NetMessageSerializedSimulation()
        {
            SerializedSimulation = serializedData
        };
        var transferOp = _sessionInterface.BeginLargeDataTransfer(netMessage, _client, description: $"Simulation-{((SimulationWorld)_simulationWorld).GetLastedTickIdFromEntity()}");
        yield return ExecuteSubOperationAndWaitForSuccess(transferOp);

        // Terminate
        TerminateWithSuccess($"Simulation sent to client.");
    }
}