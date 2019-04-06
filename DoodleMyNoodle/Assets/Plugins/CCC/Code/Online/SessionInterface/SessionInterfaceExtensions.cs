using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class SessionInterfaceExtensions
{
    public static void SendNetMessage(this SessionInterface sessionInterface, INetSerializable netMessage, params INetworkInterfaceConnection[] connections)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage(this SessionInterface sessionInterface, INetSerializable netMessage, ReadOnlyCollection<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage(this SessionInterface sessionInterface, INetSerializable netMessage, List<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
}
