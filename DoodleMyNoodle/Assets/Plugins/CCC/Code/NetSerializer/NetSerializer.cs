

using System;

public static class NetSerializer
{
    public static INetSerializerImpl impl;

    public static ushort GetTypeId(Type type)                               => impl.GetTypeId(type);
    public static Type GetTypeFromId(ushort typeId)                         => impl.GetTypeFromId(typeId);
    public static Type GetMessageType(BitStreamReader reader)               => impl.GetMessageType(reader);
    public static bool IsValidType(ushort typeId)                           => impl.IsValidType(typeId);
    public static bool IsValidType(Type type)                               => impl.IsValidType(type);
    public static int GetSerializedBitSize(object message)                  => impl.GetSerializedBitSize(message);
    public static void Serialize(object message, BitStreamWriter writer)    => impl.Serialize(message, writer);
    public static object Deserialize(BitStreamReader reader)                => impl.Deserialize(reader);
}