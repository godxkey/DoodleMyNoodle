using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMessageFactoryImpl : INetMessageFactory
{
    static Dictionary<Type, ushort> netMessageToId = new Dictionary<Type, ushort>();

    public NetMessageFactoryImpl()
    {
        netMessageToId.Clear();

        // Net message -> id
        foreach (Type netMessageType in NetMessageRegistry.types)
        {
            netMessageToId[netMessageType] = (ushort)netMessageToId.Count;
        }

        if (netMessageToId.Count > ushort.MaxValue)
        {
            Debug.LogError("To many NetMessage types for a UInt16.");
        }
    }

    public ushort GetNetMessageTypeId(NetMessage message)
    {
        return netMessageToId[message.GetType()];
    }

    public NetMessage CreateNetMessage(ushort messageType)
    {
        try
        {
            return NetMessageRegistry.factory.CreateValue(messageType);
        }
        catch
        {
            return null;
        }
    }
}
