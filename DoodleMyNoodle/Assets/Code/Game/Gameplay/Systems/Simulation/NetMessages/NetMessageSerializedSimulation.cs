
/// <summary>
/// Sent by the server to clients that need a complete simulation sync
/// </summary>
[NetSerializable]
public struct NetMessageSerializedSimulation
{
    public string SerializedSimulation;
}