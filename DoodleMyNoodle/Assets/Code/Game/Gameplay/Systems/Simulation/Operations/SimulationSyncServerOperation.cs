using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSyncServerOperation : CoroutineOperation
{
    const string LOCAL_FILE_NAME_PREFIX = "SerializedSim";

    public bool CanAcceptNewClients { get; private set; }

    List<INetworkInterfaceConnection> _clientConnections;
    SessionServerInterface _sessionInterface;

    public SimulationSyncServerOperation(SessionServerInterface sessionInterface, INetworkInterfaceConnection clientConnection)
    {
        CanAcceptNewClients = true;
        _clientConnections = new List<INetworkInterfaceConnection>
        {
            clientConnection
        };
        _sessionInterface = sessionInterface;
    }

    public void AddAdditionalClientConnection(INetworkInterfaceConnection clientConnection)
    {
        if (!CanAcceptNewClients)
            throw new Exception("cannot add new client connections");
        _clientConnections.Add(clientConnection);
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // fbessette NB: As a first iteration, the server will save the simulation to a local text file.
        //               The client (on the same PC) will load the simulation from that text file.
        //               This will only work for "local" multiplayer (server and clients on the same PC).
        //
        //               In future iterations, the server should upload the simulation directly to the other player.

        // file name example: SerializedSim-515433

        string filePath = $"{Application.persistentDataPath}/{LOCAL_FILE_NAME_PREFIX}-{SimulationView.TickId}.txt";

        // Save sim to disk
        yield return ExecuteSubOperationAndWaitForSuccess(new SaveSimulationToDiskOperation(filePath));

        // Send message to clients saying "simulation has been saved to local file, load it up"
        NetMessageSimSyncFromFile netMessageSimSyncStart = new NetMessageSimSyncFromFile()
        {
            SerializedSimulationFilePath = filePath
        };

        CanAcceptNewClients = false;
        _sessionInterface.SendNetMessage(netMessageSimSyncStart, _clientConnections);

        TerminateWithSuccess($"Simulation sent to client through file: {filePath}");

    }
}
