
/// <summary>
/// Sent by the server to clients that need a complete simulation sync
/// </summary>
[NetSerializable]
public struct NetMessageSimSyncFromFile
{
    // fbessette NB: As a first iteration, the server will actually save the simulation to a local text file.
    //               The client (on the same PC) will load the simulation from that text file.
    //               This will only work for "local" multiplayer (server and clients on the same PC).
    //
    //               In future iterations, the server should upload the simulation directly to the other player.

    public string SerializedSimulationFilePath;
}