// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class PlayerInfo
{
    public int GetNetBitSize()
    {
        int result = 0;
        result += playerName.Length * 16 + 16;
        result += playerId.GetNetBitSize();
        result += 32;
        result += 32;
        result += 16;
        result += 16;
        result += 1;
        result += 8;
        return result;
    }

    public void NetSerialize(BitStreamWriter writer)
    {
        writer.WriteString(playerName);
        playerId.NetSerialize(writer);
        writer.WriteInt32(valueInt);
        writer.WriteUInt32(valueUInt);
        writer.WriteInt16(valueShort);
        writer.WriteUInt16(valueUShort);
        writer.WriteBool(valueBool);
        writer.WriteByte(valueByte);
    }

    public void NetDeserialize(BitStreamReader reader)
    {
        playerName = reader.ReadString();
        playerId.NetDeserialize(reader);
        valueInt = reader.ReadInt32();
        valueUInt = reader.ReadUInt32();
        valueShort = reader.ReadInt16();
        valueUShort = reader.ReadUInt16();
        valueBool = reader.ReadBool();
        valueByte = reader.ReadByte();
    }
}
