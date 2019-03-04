using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class NetMessageFactory
{
    internal static INetMessageFactory instance;

    public static ushort GetNetMessageTypeId(NetMessage message) => instance.GetNetMessageTypeId(message);
    public static NetMessage CreateNetMessage(ushort messageType) => instance.CreateNetMessage(messageType);
}