using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class NetSerializer_Int32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Int32 value) => sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Int32 value, BitStreamWriter writer) => writer.WriteInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Int32 value, BitStreamReader reader) => value = reader.ReadInt32();
}
public static class NetSerializer_UInt32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref UInt32 value) => sizeof(UInt32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref UInt32 value, BitStreamWriter writer) => writer.WriteUInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref UInt32 value, BitStreamReader reader) => value = reader.ReadUInt32();
}
public static class NetSerializer_Int16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Int16 value) => sizeof(Int16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Int16 value, BitStreamWriter writer) => writer.WriteInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Int16 value, BitStreamReader reader) => value = reader.ReadInt16();
}
public static class NetSerializer_UInt16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref UInt16 value) => sizeof(UInt16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref UInt16 value, BitStreamWriter writer) => writer.WriteUInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref UInt16 value, BitStreamReader reader) => value = reader.ReadUInt16();
}
public static class NetSerializer_Byte
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Byte value) => sizeof(Byte) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Byte value, BitStreamWriter writer) => writer.WriteByte(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Byte value, BitStreamReader reader) => value = reader.ReadByte();
}
public static class NetSerializer_Single
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Single value) => sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Single value, BitStreamWriter writer) => writer.WriteFloat32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Single value, BitStreamReader reader) => value = reader.ReadFloat32();
}
public static class NetSerializer_Boolean
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Boolean value) => 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Boolean value, BitStreamWriter writer) => writer.WriteBool(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Boolean value, BitStreamReader reader) => value = reader.ReadBool();
}
public static class NetSerializer_String
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref String value) => value.Length * sizeof(Char) * 8 + sizeof(UInt16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref String value, BitStreamWriter writer) => writer.WriteString(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref String value, BitStreamReader reader) => value = reader.ReadString();
}
public static class NetSerializer_Char
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Char value) => sizeof(Char);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Char value, BitStreamWriter writer) => writer.WriteChar(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Char value, BitStreamReader reader) => value = reader.ReadChar();
}
public static class NetSerializer_Vector2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector2 value) => 2 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector2 value, BitStreamWriter writer) => writer.WriteVector2(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector2 value, BitStreamReader reader) => value = reader.ReadVector2();
}
public static class NetSerializer_Vector3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector3 value) => 3 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector3 value, BitStreamWriter writer) => writer.WriteVector3(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector3 value, BitStreamReader reader) => value = reader.ReadVector3();
}
public static class NetSerializer_Vector4
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector4 value) => 4 * sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector4 value, BitStreamWriter writer) => writer.WriteVector4(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector4 value, BitStreamReader reader) => value = reader.ReadVector4();
}
public static class NetSerializer_Vector2Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector2Int value) => 2 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector2Int value, BitStreamWriter writer) => writer.WriteVector2Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector2Int value, BitStreamReader reader) => value = reader.ReadVector2Int();
}
public static class NetSerializer_Vector3Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNetBitSize(ref Vector3Int value) => 3 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetSerialize(ref Vector3Int value, BitStreamWriter writer) => writer.WriteVector3Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NetDeserialize(ref Vector3Int value, BitStreamReader reader) => value = reader.ReadVector3Int();
}