// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial class PlayerRepertoire
{
    public int GetNetBitSize()
    {
        int result = 0;
        result += playerInfos.GetNetBitSize();
        return result;
    }

    public void NetSerialize(BitStreamWriter writer)
    {
        playerInfos.NetSerialize(writer);
    }

    public void NetDeserialize(BitStreamReader reader)
    {
        playerInfos.NetDeserialize(reader);
    }
}
