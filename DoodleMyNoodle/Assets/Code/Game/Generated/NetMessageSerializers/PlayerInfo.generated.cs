// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class PlayerInfo
{
    public override int GetNetBitSize()
    {
        int result = 0;
        result += base.GetNetBitSize();
        result += playerName.Length * 16 + 16;
        result += playerId.GetNetBitSize();
        result += 1;
        return result;
    }

    public override void NetSerialize(BitStreamWriter writer)
    {
        base.NetSerialize(writer);
        writer.WriteString(playerName);
        playerId.NetSerialize(writer);
        writer.WriteBool(isServer);
    }

    public override void NetDeserialize(BitStreamReader reader)
    {
        base.NetDeserialize(reader);
        playerName = reader.ReadString();
        playerId.NetDeserialize(reader);
        isServer = reader.ReadBool();
    }
}
