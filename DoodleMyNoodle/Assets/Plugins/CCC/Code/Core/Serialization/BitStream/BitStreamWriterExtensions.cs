using System;
using UnityEngine;

public static class BitStreamWriterExtensions
{
    public static void WriteByte(this BitStreamWriter writer, Byte value)
    {
        writer.WriteBits(value, 8);
    }

    public static void WriteChar(this BitStreamWriter writer, Char value)
    {
        writer.WriteBits(value, 16);
    }

    public static void WriteInt16(this BitStreamWriter writer, Int16 value)
    {
        writer.WriteBits(BitConverterX.Int32ToUInt32(value), 16);
    }
    public static void WriteInt32(this BitStreamWriter writer, Int32 value)
    {
        writer.WriteBits(BitConverterX.Int32ToUInt32(value), 32);
    }
    public static void WriteUInt16(this BitStreamWriter writer, UInt16 value)
    {
        writer.WriteBits(value, 16);
    }
    public static void WriteUInt32(this BitStreamWriter writer, UInt32 value)
    {
        writer.WriteBits(value, 32);
    }
    public static void WriteInt64(this BitStreamWriter writer, Int64 value)
    {
        BitConverterX.UInt64ToUInt32((UInt64)value, out UInt32 left, out UInt32 right);
        writer.WriteUInt32(left);
        writer.WriteUInt32(right);
    }
    public static void WriteUInt64(this BitStreamWriter writer, UInt64 value)
    {
        BitConverterX.UInt64ToUInt32(value, out UInt32 left, out UInt32 right);
        writer.WriteUInt32(left);
        writer.WriteUInt32(right);
    }

    public static void WriteFloat32(this BitStreamWriter writer, Single value)
    {
        writer.WriteBits(BitConverterX.Float32ToUInt32(value), 32);
    }

    public static void WriteBool(this BitStreamWriter writer, Boolean value)
    {
        writer.WriteBit(value);
    }

    public static void WriteString(this BitStreamWriter writer, string value)
    {
        writer.WriteUInt32((UInt32)value.Length);

        for (Int32 i = 0; i < value.Length; i++)
        {
            writer.WriteChar(value[i]);
        }
    }




    public static void WriteVector2(this BitStreamWriter writer, Vector2 value)
    {
        writer.WriteFloat32(value.x);
        writer.WriteFloat32(value.y);
    }
    public static void WriteVector3(this BitStreamWriter writer, Vector3 value)
    {
        writer.WriteFloat32(value.x);
        writer.WriteFloat32(value.y);
        writer.WriteFloat32(value.z);
    }
    public static void WriteVector4(this BitStreamWriter writer, Vector4 value)
    {
        writer.WriteFloat32(value.x);
        writer.WriteFloat32(value.y);
        writer.WriteFloat32(value.z);
        writer.WriteFloat32(value.w);
    }

    public static void WriteVector2Int(this BitStreamWriter writer, Vector2Int value)
    {
        writer.WriteInt32(value.x);
        writer.WriteInt32(value.y);
    }
    public static void WriteVector3Int(this BitStreamWriter writer, Vector3Int value)
    {
        writer.WriteInt32(value.x);
        writer.WriteInt32(value.y);
        writer.WriteInt32(value.z);
    }

}
