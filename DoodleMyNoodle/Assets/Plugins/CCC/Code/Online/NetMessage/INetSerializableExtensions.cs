using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class INetSerializableExtensions
{
    //public static int GetNetBitSize(this List<int> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<uint> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<short> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<ushort> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<long> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<ulong> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<float> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<bool> list) => 32 + list.Count * sizeof(int) * 8;
    //public static int GetNetBitSize(this List<string> list) => 32 + list.Count * sizeof(int) * 8;

    public static int GetNetBitSize<T>(this List<T> list) where T : INetSerializable
    {
        int result = 32; // list size
        for (int i = 0; i < list.Count; i++)
        {
            result += list[i].GetNetBitSize();
        }
        return result;
    }

    public static void NetSerialize<T>(this List<T> list, BitStreamWriter writer) where T : INetSerializable
    {
        writer.WriteInt32(list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].NetSerialize(writer);
        }
    }

    public static void NetDeserialize<T>(this List<T> list, BitStreamReader reader) where T : INetSerializable, new()
    {
        int count = reader.ReadInt32();

        if (list.Capacity < count)
            list.Capacity = count * 2;

        for (int i = 0; i < count; i++)
        {
            if (i == list.Count)
                list.Add(new T());
            list[i].NetDeserialize(reader);
        }

        if (list.Count > count)
        {
            list.RemoveRange(count, list.Count - count);
        }
    }
}
