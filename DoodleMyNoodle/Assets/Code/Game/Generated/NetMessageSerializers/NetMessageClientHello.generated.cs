// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class NetMessageClientHello
{
    public override int GetNetBitSize()
    {
        int result = 0;
        result += base.GetNetBitSize();
        result += playerName.Length * 16 + 16;
        return result;
    }

    public override void NetSerialize(BitStreamWriter writer)
    {
        base.NetSerialize(writer);
        writer.WriteString(playerName);
    }

    public override void NetDeserialize(BitStreamReader reader)
    {
        base.NetDeserialize(reader);
        playerName = reader.ReadString();
    }
}
