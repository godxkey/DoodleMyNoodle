using System;
using UnityEngine;

public static class BitStreamWriterExtensions
{
    public static void WriteByte(this BitStreamWriter writer, byte value)
    {
        writer.WriteBits(value, 8);
    }

    public static void WriteChar(this BitStreamWriter writer, char value)
    {
        writer.WriteBits(value, 16);
    }

    public static void WriteInt16(this BitStreamWriter writer, short value)
    {
        writer.WriteBits(BitConverterX.Int32ToUInt32(value), 16);
    }
    public static void WriteInt32(this BitStreamWriter writer, int value)
    {
        writer.WriteBits(BitConverterX.Int32ToUInt32(value), 32);
    }
    public static void WriteUInt16(this BitStreamWriter writer, ushort value)
    {
        writer.WriteBits(value, 16);
    }
    public static void WriteUInt32(this BitStreamWriter writer, uint value)
    {
        writer.WriteBits(value, 32);
    }

    public static void WriteFloat32(this BitStreamWriter writer, float value)
    {
        writer.WriteBits(BitConverterX.Float32ToUInt32(value), 32);
    }

    public static void WriteBool(this BitStreamWriter writer, bool value)
    {
        writer.WriteBit(value);
    }

    public static void WriteString(this BitStreamWriter writer, string value)
    {
        writer.WriteUInt16((UInt16)value.Length);

        for (int i = 0; i < value.Length; i++)
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
