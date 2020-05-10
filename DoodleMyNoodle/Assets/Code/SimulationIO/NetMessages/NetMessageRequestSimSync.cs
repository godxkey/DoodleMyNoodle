
/// <summary>
/// Sent by the client to the server to request a complete simulation sync
/// </summary>
[NetSerializable]
public struct NetMessageRequestSimSync
{
    public bool AttemptTransferByDisk;
    public string LocalMachineName;
}


/// <summary>
/// Sent by the client to the server to request a complete simulation sync
/// </summary>
[NetSerializable]
public struct NetMessageAcceptSimSync
{
    // used to speed up the transfer if the server and the client are on the same PC
    public bool TransferByDisk;
}