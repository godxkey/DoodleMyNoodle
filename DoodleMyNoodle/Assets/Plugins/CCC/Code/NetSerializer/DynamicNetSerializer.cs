

using System;

public static class DynamicNetSerializer
{
    public static IDynamicNetSerializerImpl impl;

    public static ushort GetTypeId(Type type)                               => impl.GetTypeId(type);
    public static Type GetTypeFromId(ushort typeId)                         => impl.GetTypeFromId(typeId);
    public static Type GetMessageType(BitStreamReader reader)               => impl.GetMessageType(reader);
    public static bool IsValidType(ushort typeId)                           => impl.IsValidType(typeId);
    public static bool IsValidType(Type type)                               => impl.IsValidType(type);
    public static int GetNetBitSize(object message)                         => impl.GetNetBitSize(message);
    public static void NetSerialize(object message, BitStreamWriter writer) => impl.NetSerialize(message, writer);
    public static object NetDeserialize(BitStreamReader reader)             => impl.NetDeserialize(reader);
}