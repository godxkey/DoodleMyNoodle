using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSyncClientOperation : CoroutineOperation
{
    public bool IsServerReadyToSendSimTicksForSimulationWereSyncingToo { get; private set; }

    SessionClientInterface _sessionInterface;
    string _simLoadFile;
    bool _receivedResponseFromServer;

    public SimulationSyncClientOperation(SessionClientInterface sessionInterface)
    {
        _sessionInterface = sessionInterface;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // fbessette NB: As a first iteration, the server will save the simulation to a local text file.
        //               The client (on the same PC) will load the simulation from that text file.
        //               This will only work for "local" multiplayer (server and clients on the same PC).
        //
        //               In future iterations, the server should upload the simulation directly to the other player.

        // file name example: SerializedSim-515433
        

        // request sync to server
        NetMessageRequestSimSync syncRequest = new NetMessageRequestSimSync();
        _sessionInterface.SendNetMessageToServer(syncRequest);

        // wait for server response
        _sessionInterface.RegisterNetMessageReceiver<NetMessageSimSyncFromFile>(OnNetMessageSimSyncFromFile);

        // wait for _loadPath to be assigned
        while (_receivedResponseFromServer == false)
        {
            yield return null;
        }
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageSimSyncFromFile>(OnNetMessageSimSyncFromFile);

        // terminate if load path is invalid
        if(_simLoadFile.IsNullOrEmpty())
        {
            TerminateWithFailure($"Invalid simulation load file path received from the server ({_simLoadFile}). Prehaps it failed to serialize it.");
            yield break;
        }

        yield return ExecuteSubOperationAndWaitForSuccess(new LoadSimulationFromDiskOperation(_simLoadFile));

        TerminateWithSuccess();
    }

    void OnNetMessageSimSyncFromFile(NetMessageSimSyncFromFile netMessage, INetworkInterfaceConnection source)
    {
        _receivedResponseFromServer = true;
        _simLoadFile = netMessage.SerializedSimulationFilePath;
        IsServerReadyToSendSimTicksForSimulationWereSyncingToo = true;
    }

    protected override void OnTerminate()
    {
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageSimSyncFromFile>(OnNetMessageSimSyncFromFile);
    }
}
