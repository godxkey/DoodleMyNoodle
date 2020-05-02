using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Mathematics;

public static class StaticNetSerializer_System_Int64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Int64 value) => sizeof(Int64) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(Int64) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Int64 value, BitStreamWriter writer) => writer.WriteInt64(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Int64 value, BitStreamWriter writer) => writer.WriteInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Int64 value, BitStreamReader reader) => value = reader.ReadInt64();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 NetDeserialize(BitStreamReader reader) => reader.ReadInt64();
}
public static class StaticNetSerializer_System_UInt64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref UInt64 value) => sizeof(UInt64) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref UInt64 value, BitStreamWriter writer) => writer.WriteUInt64(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref UInt64 value, BitStreamReader reader) => value = reader.ReadUInt64();
}
public static class StaticNetSerializer_System_Int32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Int32 value) => sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(Int32) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Int32 value, BitStreamWriter writer) => writer.WriteInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Int32 value, BitStreamWriter writer) => writer.WriteInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Int32 value, BitStreamReader reader) => value = reader.ReadInt32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 NetDeserialize(BitStreamReader reader) => reader.ReadInt32();
}
public static class StaticNetSerializer_System_UInt32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref UInt32 value) => sizeof(UInt32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(UInt32) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref UInt32 value, BitStreamWriter writer) => writer.WriteUInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(UInt32 value, BitStreamWriter writer) => writer.WriteUInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref UInt32 value, BitStreamReader reader) => value = reader.ReadUInt32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 NetDeserialize(BitStreamReader reader) => reader.ReadUInt32();
}
public static class StaticNetSerializer_System_Int16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Int16 value) => sizeof(Int16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(Int16) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Int16 value, BitStreamWriter writer) => writer.WriteInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Int16 value, BitStreamWriter writer) => writer.WriteInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Int16 value, BitStreamReader reader) => value = reader.ReadInt16();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int16 NetDeserialize(BitStreamReader reader) => reader.ReadInt16();
}
public static class StaticNetSerializer_System_UInt16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref UInt16 value) => sizeof(UInt16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(UInt16) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref UInt16 value, BitStreamWriter writer) => writer.WriteUInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(UInt16 value, BitStreamWriter writer) => writer.WriteUInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref UInt16 value, BitStreamReader reader) => value = reader.ReadUInt16();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt16 NetDeserialize(BitStreamReader reader) => reader.ReadUInt16();
}
public static class StaticNetSerializer_System_Byte
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Byte value) => sizeof(Byte) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(Byte) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Byte value, BitStreamWriter writer) => writer.WriteByte(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Byte value, BitStreamWriter writer) => writer.WriteByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Byte value, BitStreamReader reader) => value = reader.ReadByte();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte NetDeserialize(BitStreamReader reader) => reader.ReadByte();
}
public static class StaticNetSerializer_System_Single
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Single value) => sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => sizeof(Single) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Single value, BitStreamWriter writer) => writer.WriteFloat32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Single value, BitStreamWriter writer) => writer.WriteFloat32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Single value, BitStreamReader reader) => value = reader.ReadFloat32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single NetDeserialize(BitStreamReader reader) => reader.ReadFloat32();
}
public static class StaticNetSerializer_System_Boolean
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Boolean value) => 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize() => 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Boolean value, BitStreamWriter writer) => writer.WriteBool(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(Boolean value, BitStreamWriter writer) => writer.WriteBool(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Boolean value, BitStreamReader reader) => value = reader.ReadBool();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean NetDeserialize(BitStreamReader reader) => reader.ReadBool();
}
public static class StaticNetSerializer_System_String
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref String value)
    {
        if (value != null)
        {
            return 1 + (value.Length * sizeof(Char) * 8 + sizeof(UInt32) * 8);
        }
        else
        {
            return 1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref String value, BitStreamWriter writer)
    {
        if (value != null)
        {
            writer.WriteBit(true);
            writer.WriteString(value);
        }
        else
        {
            writer.WriteBit(false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref String value, BitStreamReader reader)
    {
        if (reader.ReadBit())
        {
            value = reader.ReadString();
        }
        else
        {
            value = null;
        }
    }
}
public static class StaticNetSerializer_System_Char
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Char value) => sizeof(Char);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Char value, BitStreamWriter writer) => writer.WriteChar(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Char value, BitStreamReader reader) => value = reader.ReadChar();
}
public static class StaticNetSerializer_UnityEngine_Vector2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector2 value) => 2 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector2 value, BitStreamWriter writer) => writer.WriteVector2(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector2 value, BitStreamReader reader) => value = reader.ReadVector2();
}
public static class StaticNetSerializer_UnityEngine_Vector3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector3 value) => 3 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector3 value, BitStreamWriter writer) => writer.WriteVector3(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector3 value, BitStreamReader reader) => value = reader.ReadVector3();
}
public static class StaticNetSerializer_UnityEngine_Vector4
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector4 value) => 4 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector4 value, BitStreamWriter writer) => writer.WriteVector4(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector4 value, BitStreamReader reader) => value = reader.ReadVector4();
}
public static class StaticNetSerializer_UnityEngine_Vector2Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector2Int value) => 2 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector2Int value, BitStreamWriter writer) => writer.WriteVector2Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector2Int value, BitStreamReader reader) => value = reader.ReadVector2Int();
}
public static class StaticNetSerializer_UnityEngine_Vector3Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector3Int value) => 3 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector3Int value, BitStreamWriter writer) => writer.WriteVector3Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector3Int value, BitStreamReader reader) => value = reader.ReadVector3Int();
}

public static class StaticNetSerializer_System_Object
{
    public static int GetNetBitSize_Class(object obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static void NetSerialize_Class(object obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }

    public static object NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return DynamicNetSerializer.NetDeserialize(reader);
    }
}

public static class StaticNetSerializer_Unity_Mathematics_int2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref int2 value) => 2 * sizeof(Int32) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref int2 value, BitStreamWriter writer)
    {
        writer.WriteInt32(value.x);
        writer.WriteInt32(value.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref int2 value, BitStreamReader reader)
    {
        value.x = reader.ReadInt32();
        value.y = reader.ReadInt32();
    }
}
