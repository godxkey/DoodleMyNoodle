using System;
using System.Text;
using UnityEngine;

public static class BitStreamReaderExtensions
{
    public static Byte ReadByte(this BitStreamReader reader)
    {
        return (Byte)reader.ReadBits(8);
    }

    public static Char ReadChar(this BitStreamReader reader)
    {
        return (Char)reader.ReadBits(16);
    }

    public static Int16 ReadInt16(this BitStreamReader reader)
    {
        return (Int16)reader.ReadBits(16);
    }
    public static Int32 ReadInt32(this BitStreamReader reader)
    {
        return reader.ReadBits(32);
    }
    public static UInt16 ReadUInt16(this BitStreamReader reader)
    {
        return (UInt16)BitConverterX.Int32ToUInt32(reader.ReadBits(16));
    }
    public static UInt32 ReadUInt32(this BitStreamReader reader)
    {
        return BitConverterX.Int32ToUInt32(reader.ReadBits(32));
    }
    public static Int64 ReadInt64(this BitStreamReader reader)
    {
        UInt32 left = reader.ReadUInt32();
        UInt32 right = reader.ReadUInt32();
        return (Int64)BitConverterX.UInt32ToUInt64(left, right);
    }
    public static UInt64 ReadUInt64(this BitStreamReader reader)
    {
        UInt32 left = reader.ReadUInt32();
        UInt32 right = reader.ReadUInt32();
        return BitConverterX.UInt32ToUInt64(left, right);
    }

    public static Single ReadFloat32(this BitStreamReader reader)
    {
        return BitConverterX.Int32ToFloat32(reader.ReadBits(32));
    }

    public static Boolean ReadBool(this BitStreamReader reader)
    {
        return reader.ReadBit();
    }

    public static string ReadString(this BitStreamReader reader)
    {
        UInt32 size = reader.ReadUInt32();
        StringBuilder stringBuilder = new StringBuilder((int)size);

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
