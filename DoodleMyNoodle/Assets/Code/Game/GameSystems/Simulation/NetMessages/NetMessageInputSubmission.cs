
/// <summary>
/// Sent by the client when it wants to submit an input in the simulation
/// </summary>
[NetSerializable]
public struct NetMessageInputSubmission
{
    /// <summary>
    /// The submissionId IS NOT UNIQUE across clients/server, it is only unique for a given client
    /// (e.g. ClientA and ClientB might have a submission #59) 
    /// This is used to calculate round trip time (RTT)
    /// </summary>
    public InputSubmissionId submissionId;

    /// <summary>
    /// Input in question
    /// </summary>
    public SimInput input;
}