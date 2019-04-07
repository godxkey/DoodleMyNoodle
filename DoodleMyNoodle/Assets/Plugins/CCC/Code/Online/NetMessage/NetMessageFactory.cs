using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class NetMessageFactory
{
    internal static INetMessageFactoryImpl impl;

    public static ushort GetNetMessageTypeId(object message)                                        => impl.GetNetMessageTypeId(message);
    public static int GetMessageBitSize(ushort messageType, object message)                         => impl.GetMessageBitSize(messageType, message);
    public static void SerializeMessage(ushort messageType, object message, BitStreamWriter writer) => impl.SerializeMessage(messageType, message, writer);
    public static object DeserializeMessage(ushort messageType, BitStreamReader reader)             => impl.DeserializeMessage(messageType, reader);
}