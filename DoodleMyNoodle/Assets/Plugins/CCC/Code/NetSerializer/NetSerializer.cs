using System;
using System.Collections.Concurrent;
using Unity.Mathematics;
using UnityEngineX;

public static class NetSerializer
{
    public static INetSerializerImpl impl;

    public static ushort GetTypeId(Type type) => impl.GetTypeId(type);
    public static Type GetTypeFromId(ushort typeId) => impl.GetTypeFromId(typeId);
    public static Type GetObjectType(BitStreamReader reader) => impl.GetObjectType(reader);
    public static bool IsValidType(ushort typeId) => impl.IsValidType(typeId);
    public static bool IsValidType(Type type) => impl.IsValidType(type);
    public static int GetSerializedBitSize(object obj) => impl.GetSerializedBitSize(obj);
    public static void Serialize(object obj, BitStreamWriter writer) => impl.Serialize(obj, writer);
    public static object Deserialize(BitStreamReader reader) => impl.Deserialize(reader);

    static ConcurrentPool<BitStreamReader> s_readers = new ConcurrentPool<BitStreamReader>(() => new BitStreamReader(null));
    static ConcurrentPool<BitStreamWriter> s_writers = new ConcurrentPool<BitStreamWriter>(() => new BitStreamWriter(null));

    public static Type GetObjectType(byte[] messageData)
    {
        using (s_readers.GetScoped(out BitStreamReader reader))
        {
            reader.SetNewBuffer(messageData);
            return GetObjectType(reader);
        }
    }

    public static T Deserialize<T>(byte[] messageData)
    {
        return (T)Deserialize(messageData);
    }

    public static object Deserialize(byte[] data)
    {
        using (s_readers.GetScoped(out BitStreamReader reader))
        {
            reader.SetNewBuffer(data);
            return Deserialize(reader);
        }
    }

    public static byte[] Serialize<T>(T obj)
    {
        int netBitSize = GetSerializedBitSize(obj);
        int messageSizeByte = mathX.ceil(netBitSize, 8) / 8; // this will ceil the size to a multiple of 8

        var data = new byte[messageSizeByte];

        using (s_writers.GetScoped(out BitStreamWriter writer))
        {
            writer.SetNewBuffer(data);
            Serialize(obj, writer);
        }

        return data;
    }

    private static class mathX
    {
        public static int ceil(int value, int step)
        {
            int extra = value % step;

            if (extra != 0 && math.sign(extra) == math.sign(step))
                extra -= step;

            return value - extra;
        }
    }
}
