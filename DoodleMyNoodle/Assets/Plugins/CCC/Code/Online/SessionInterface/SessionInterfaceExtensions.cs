using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class SessionInterfaceExtensions
{
    public static void SendNetMessage(this SessionInterface sessionInterface, object netMessage, params INetworkInterfaceConnection[] connections)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage(this SessionInterface sessionInterface, object netMessage, ReadOnlyCollection<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage(this SessionInterface sessionInterface, object netMessage, List<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
}
