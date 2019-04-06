// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class NetMessagePlayerRepertoireSync
{
    public override int GetNetBitSize()
    {
        int result = 0;
        result += base.GetNetBitSize();
        result += players.GetNetBitSize();
        return result;
    }

    public override void NetSerialize(BitStreamWriter writer)
    {
        base.NetSerialize(writer);
        players.NetSerialize(writer);
    }

    public override void NetDeserialize(BitStreamReader reader)
    {
        base.NetDeserialize(reader);
        players.NetDeserialize(reader);
    }
}
