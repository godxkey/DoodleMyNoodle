using System;
using System.Text;
using UnityEngine;

public static class BitStreamReaderExtensions
{
    public static byte ReadByte(this BitStreamReader reader)
    {
        return (byte)reader.ReadBits(8);
    }

    public static char ReadChar(this BitStreamReader reader)
    {
        return (char)reader.ReadBits(16);
    }

    public static short ReadInt16(this BitStreamReader reader)
    {
        return (short)reader.ReadBits(16);
    }
    public static int ReadInt32(this BitStreamReader reader)
    {
        return reader.ReadBits(32);
    }
    public static ushort ReadUInt16(this BitStreamReader reader)
    {
        return (ushort)BitConverterX.Int32ToUInt32(reader.ReadBits(16));
    }
    public static uint ReadUInt32(this BitStreamReader reader)
    {
        return BitConverterX.Int32ToUInt32(reader.ReadBits(32));
    }

    public static float ReadFloat32(this BitStreamReader reader)
    {
        return BitConverterX.Int32ToFloat32(reader.ReadBits(32));
    }

    public static bool ReadBool(this BitStreamReader reader)
    {
        return reader.ReadBit();
    }

    public static string ReadString(this BitStreamReader reader)
    {
        UInt16 size = reader.ReadUInt16();
        StringBuilder stringBuilder = new StringBuilder(size);

        for (int i = 0; i < size; i++)
        {
            stringBuilder.Append(reader.ReadChar());
        }

        return stringBuilder.ToString();
    }

    public static Vector2 ReadVector2(this BitStreamReader reader)
    {
        return new Vector2
        {
            x = reader.ReadFloat32(),
            y = reader.ReadFloat32()
        };
    }
    public static Vector3 ReadVector3(this BitStreamReader reader)
    {
        return new Vector3
        {
            x = reader.ReadFloat32(),
            y = reader.ReadFloat32(),
            z = reader.ReadFloat32()
        };
    }
    public static Vector4 ReadVector4(this BitStreamReader reader)
    {
        return new Vector4
        {
            x = reader.ReadFloat32(),
            y = reader.ReadFloat32(),
            z = reader.ReadFloat32(),
            w = reader.ReadFloat32()
        };
    }

    public static Vector2Int ReadVector2Int(this BitStreamReader reader)
    {
        return new Vector2Int
        {
            x = reader.ReadInt32(),
            y = reader.ReadInt32()
        };
    }
    public static Vector3Int ReadVector3Int(this BitStreamReader reader)
    {
        return new Vector3Int
        {
            x = reader.ReadInt32(),
            y = reader.ReadInt32(),
            z = reader.ReadInt32()
        };
    }

}
