// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class NetMessageExample
{
    public override int GetNetBitSize()
    {
        int result = 0;
        result += base.GetNetBitSize();
        result += valueString.Length * 16 + 16;
        result += 32;
        result += 32;
        result += 16;
        result += 16;
        result += 1;
        result += 8;
        result += 16; // list size
        for (int listOfNetSerializableValue_i = 0; listOfNetSerializableValue_i < listOfNetSerializableValue.Count; listOfNetSerializableValue_i++)
        {
            result += 32;
        }
        return result;
    }

    public override void NetSerialize(BitStreamWriter writer)
    {
        base.NetSerialize(writer);
        writer.WriteString(valueString);
        writer.WriteInt32(valueInt);
        writer.WriteUInt32(valueUInt);
        writer.WriteInt16(valueShort);
        writer.WriteUInt16(valueUShort);
        writer.WriteBool(valueBool);
        writer.WriteByte(valueByte);
        writer.WriteUInt16((UInt16)listOfNetSerializableValue.Count);
        for (ushort listOfNetSerializableValue_i = 0; listOfNetSerializableValue_i < listOfNetSerializableValue.Count; listOfNetSerializableValue_i++)
        {
            writer.WriteInt32(listOfNetSerializableValue[listOfNetSerializableValue_i]);
        }
    }

    public override void NetDeserialize(BitStreamReader reader)
    {
        base.NetDeserialize(reader);
        valueString = reader.ReadString();
        valueInt = reader.ReadInt32();
        valueUInt = reader.ReadUInt32();
        valueShort = reader.ReadInt16();
        valueUShort = reader.ReadUInt16();
        valueBool = reader.ReadBool();
        valueByte = reader.ReadByte();
        int listOfNetSerializableValue_count = reader.ReadUInt16();
        if (listOfNetSerializableValue.Capacity < listOfNetSerializableValue_count)
            listOfNetSerializableValue.Capacity = listOfNetSerializableValue_count * 2;
        for (ushort listOfNetSerializableValue_i = 0; listOfNetSerializableValue_i < listOfNetSerializableValue_count; listOfNetSerializableValue_i++)
        {
            if (listOfNetSerializableValue_i == listOfNetSerializableValue.Count)
                listOfNetSerializableValue.Add(new System.Int32());
            listOfNetSerializableValue[listOfNetSerializableValue_i] = reader.ReadInt32();
        }
        if(listOfNetSerializableValue.Count > listOfNetSerializableValue_count)
        {
            listOfNetSerializableValue.RemoveRange(listOfNetSerializableValue_count, listOfNetSerializableValue.Count - listOfNetSerializableValue_count);
        }
    }
}
