using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngineX;

public static class SessionInterfaceExtensions
{
    public static void SendNetMessage<T>(this SessionInterface sessionInterface, in T netMessage, params INetworkInterfaceConnection[] connections)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage<T>(this SessionInterface sessionInterface, in T netMessage, in ReadOnlyList<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
    public static void SendNetMessage<T>(this SessionInterface sessionInterface, in T netMessage, List<INetworkInterfaceConnection> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == null)
                continue;
            sessionInterface.SendNetMessage(netMessage, connections[i]);
        }
    }
}
