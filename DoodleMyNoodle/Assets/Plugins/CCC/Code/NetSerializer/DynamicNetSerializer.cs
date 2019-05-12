

public static class DynamicNetSerializer
{
    public static IDynamicNetSerializerImpl impl;

    public static int GetNetBitSize(object message)                         => impl.GetNetBitSize(message);
    public static void NetSerialize(object message, BitStreamWriter writer) => impl.NetSerialize(message, writer);
    public static object NetDeserialize(BitStreamReader reader)             => impl.NetDeserialize(reader);
}