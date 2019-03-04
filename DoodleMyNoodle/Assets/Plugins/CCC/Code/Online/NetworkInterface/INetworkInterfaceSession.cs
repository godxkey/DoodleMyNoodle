

using System;

public interface INetworkInterfaceSession
{
    Guid Id { get; }
    int ConnectionsMax { get; }
    int ConnectionsCurrent { get; }
    string HostName { get; }
}
