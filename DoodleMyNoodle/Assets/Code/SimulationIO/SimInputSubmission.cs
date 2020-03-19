
/// <summary>
/// A SimInput approved by the server that will be sent to the simulation
/// </summary>
[NetSerializable]
public struct SimInputSubmission
{
    public uint InstigatorConnectionId;

    /// <summary>
    /// The submissionId IS NOT UNIQUE across clients/server, it is only unique for a given client
    /// (e.g. ClientA and ClientB might have a submission #59) 
    /// This is used to calculate round trip time (RTT)
    /// </summary>
    public InputSubmissionId ClientSubmissionId;

    /// <summary>
    /// The input to feed into the simulation
    /// </summary>
    public SimInput Input;
}