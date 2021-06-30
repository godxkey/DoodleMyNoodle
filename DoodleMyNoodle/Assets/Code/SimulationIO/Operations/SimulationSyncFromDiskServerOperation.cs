using CCC.Operations;
using System.Collections;
using Unity.Entities;
using UnityEngine;

public class SimulationSyncFromDiskServerOperation : CoroutineOperation
{
    const string LOCAL_FILE_NAME_PREFIX = "SerializedSim";

    private INetworkInterfaceConnection _client;
    private SessionServerInterface _sessionInterface;
    private World _simulationWorld;

    public SimulationSyncFromDiskServerOperation(SessionServerInterface sessionInterface, INetworkInterfaceConnection clientConnection, World simulationWorld)
    {
        _client = clientConnection;
        _sessionInterface = sessionInterface;
        _simulationWorld = simulationWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // The server will save the simulation to a local text file.
        // The client (on the same PC) will load the simulation from that text file.
        // This will only work for "local" multiplayer (server and clients on the same PC).

        // file name example: SerializedSim-515433

        string filePath;
        if (_simulationWorld is SimulationWorld simWorld)
        {
            var latestTick = simWorld.GetLastTickIdFromEntity();

            filePath = $"{Application.persistentDataPath}/{LOCAL_FILE_NAME_PREFIX}-{latestTick}.txt";
        }
        else
        {
            filePath = $"{Application.persistentDataPath}/{LOCAL_FILE_NAME_PREFIX}-.txt";
        }

        // Save sim to disk
        yield return ExecuteSubOperationAndWaitForSuccess(new SaveSimulationToDiskOperation(filePath, _simulationWorld));

        // Send message to clients saying "simulation has been saved to local file, load it up"
        NetMessageSimSyncFromFile netMessageSimSyncStart = new NetMessageSimSyncFromFile()
        {
            SerializedSimulationFilePath = filePath
        };

        _sessionInterface.SendNetMessage(netMessageSimSyncStart, _client);

        TerminateWithSuccess($"Simulation sent to client through file: {filePath}");
    }
}