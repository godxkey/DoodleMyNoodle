using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Mathematics;
using UnityEngineX;

public static class StaticNetSerializer_System_Int64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Int64 value) => sizeof(Int64) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(Int64) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Int64 value, BitStreamWriter writer) => writer.WriteInt64(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Int64 value, BitStreamWriter writer) => writer.WriteInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Int64 value, BitStreamReader reader) => value = reader.ReadInt64();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 Deserialize(BitStreamReader reader) => reader.ReadInt64();
}
public static class StaticNetSerializer_System_UInt64
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref UInt64 value) => sizeof(UInt64) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref UInt64 value, BitStreamWriter writer) => writer.WriteUInt64(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref UInt64 value, BitStreamReader reader) => value = reader.ReadUInt64();
}
public static class StaticNetSerializer_System_Int32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Int32 value) => sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(Int32) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Int32 value, BitStreamWriter writer) => writer.WriteInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Int32 value, BitStreamWriter writer) => writer.WriteInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Int32 value, BitStreamReader reader) => value = reader.ReadInt32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Deserialize(BitStreamReader reader) => reader.ReadInt32();
}
public static class StaticNetSerializer_System_UInt32
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref UInt32 value) => sizeof(UInt32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(UInt32) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref UInt32 value, BitStreamWriter writer) => writer.WriteUInt32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(UInt32 value, BitStreamWriter writer) => writer.WriteUInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref UInt32 value, BitStreamReader reader) => value = reader.ReadUInt32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 Deserialize(BitStreamReader reader) => reader.ReadUInt32();
}
public static class StaticNetSerializer_System_Int16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Int16 value) => sizeof(Int16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(Int16) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Int16 value, BitStreamWriter writer) => writer.WriteInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Int16 value, BitStreamWriter writer) => writer.WriteInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Int16 value, BitStreamReader reader) => value = reader.ReadInt16();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int16 Deserialize(BitStreamReader reader) => reader.ReadInt16();
}
public static class StaticNetSerializer_System_UInt16
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref UInt16 value) => sizeof(UInt16) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(UInt16) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref UInt16 value, BitStreamWriter writer) => writer.WriteUInt16(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(UInt16 value, BitStreamWriter writer) => writer.WriteUInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref UInt16 value, BitStreamReader reader) => value = reader.ReadUInt16();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt16 Deserialize(BitStreamReader reader) => reader.ReadUInt16();
}
public static class StaticNetSerializer_System_Byte
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Byte value) => sizeof(Byte) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(Byte) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Byte value, BitStreamWriter writer) => writer.WriteByte(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Byte value, BitStreamWriter writer) => writer.WriteByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Byte value, BitStreamReader reader) => value = reader.ReadByte();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte Deserialize(BitStreamReader reader) => reader.ReadByte();
}
public static class StaticNetSerializer_System_Single
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Single value) => sizeof(Single) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => sizeof(Single) * 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Single value, BitStreamWriter writer) => writer.WriteFloat32(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Single value, BitStreamWriter writer) => writer.WriteFloat32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Single value, BitStreamReader reader) => value = reader.ReadFloat32();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single Deserialize(BitStreamReader reader) => reader.ReadFloat32();
}
public static class StaticNetSerializer_System_Boolean
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Boolean value) => 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize() => 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Boolean value, BitStreamWriter writer) => writer.WriteBool(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(Boolean value, BitStreamWriter writer) => writer.WriteBool(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Boolean value, BitStreamReader reader) => value = reader.ReadBool();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean Deserialize(BitStreamReader reader) => reader.ReadBool();
}
public static class StaticNetSerializer_System_String
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref String value)
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
    public static void Serialize(ref String value, BitStreamWriter writer)
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
    public static void Deserialize(ref String value, BitStreamReader reader)
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
    public static int GetSerializedBitSize(ref Char value) => sizeof(Char);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Char value, BitStreamWriter writer) => writer.WriteChar(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Char value, BitStreamReader reader) => value = reader.ReadChar();
}
public static class StaticNetSerializer_UnityEngine_Vector2Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Vector2Int value) => 2 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Vector2Int value, BitStreamWriter writer) => writer.WriteVector2Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Vector2Int value, BitStreamReader reader) => value = reader.ReadVector2Int();
}
public static class StaticNetSerializer_UnityEngine_Vector3Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Vector3Int value) => 3 * sizeof(Int32) * 8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Vector3Int value, BitStreamWriter writer) => writer.WriteVector3Int(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Vector3Int value, BitStreamReader reader) => value = reader.ReadVector3Int();
}

public static class StaticNetSerializer_System_Object
{
    public static int GetSerializedBitSize_Class(object obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static void Serialize_Class(object obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }

    public static object Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return NetSerializer.Deserialize(reader);
    }
}

public static class StaticNetSerializer_System_Guid
{
    private const int BYTE_COUNT = 16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Guid _) => BYTE_COUNT * 8;

    public static void Serialize(ref Guid value, BitStreamWriter writer)
    {
        byte[] b = value.ToByteArray();
        
        if (b.Length != BYTE_COUNT)
            throw new Exception("wat?");
        
        for (int i = 0; i < BYTE_COUNT; i++)
        {
            writer.WriteByte(b[i]);
        }
    }

    public static void Deserialize(ref Guid value, BitStreamReader reader)
    {
        byte[] b = new byte[BYTE_COUNT];

        for (int i = 0; i < BYTE_COUNT; i++)
        {
            b[i] = reader.ReadByte();
        }

        value = new Guid(b);
    }
}

public static class StaticNetSerializer_System_DateTime
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref DateTime _) => sizeof(long) * 8;

    public static void Serialize(ref DateTime value, BitStreamWriter writer)
    {
        Log.Assert(value.Kind == DateTimeKind.Utc, "DateTime must be UTC");
        writer.WriteInt64(value.Ticks);
    }

    public static void Deserialize(ref DateTime value, BitStreamReader reader)
    {
        value = new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
    }
}