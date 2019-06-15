
/// <summary>
/// A SimInput approved by the server that will be sent to the simulation
/// </summary>
[NetSerializable]
public struct ApprovedSimInput
{
    /// <summary>
    /// The player that submitted the input in the first place
    /// </summary>
    public PlayerId playerInstigator;

    /// <summary>
    /// The submissionId IS NOT UNIQUE across clients/server, it is only unique for a given client
    /// (e.g. ClientA and ClientB might have a submission #59) 
    /// This is used to calculate round trip time (RTT)
    /// </summary>
    public InputSubmissionId clientSubmissionId;

    /// <summary>
    /// The input to feed into the simulation
    /// </summary>
    public SimInput input;
}