
/// <summary>
/// Sent by the server to all clients for each processed simulation tick
/// </summary>
[NetSerializable]
public struct NetMessageSimTick
{
    /// <summary>
    /// The next tickId of the simulation. This is mainly used for validation (make sure the client didn't skip anything)
    /// </summary>
    public uint tickId;

    /// <summary>
    /// All of the inputs to process in the simulation tick
    /// </summary>
    public ApprovedSimInput[] inputs;
}