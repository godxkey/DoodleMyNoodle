// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class NetMessagePlayerJoined
{
    public override int GetNetBitSize()
    {
        int result = 0;
        result += base.GetNetBitSize();
        result += playerInfo.GetNetBitSize();
        return result;
    }

    public override void NetSerialize(BitStreamWriter writer)
    {
        base.NetSerialize(writer);
        playerInfo.NetSerialize(writer);
    }

    public override void NetDeserialize(BitStreamReader reader)
    {
        base.NetDeserialize(reader);
        playerInfo.NetDeserialize(reader);
    }
}
