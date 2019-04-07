// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessageClientHello
{
    public static int GetNetBitSize(ref NetMessageClientHello obj)
    {
        int result = 0;
        result += NetSerializer_String.GetNetBitSize(ref obj.playerName);
        return result;
    }

    public static void NetSerialize(ref NetMessageClientHello obj, BitStreamWriter writer)
    {
        NetSerializer_String.NetSerialize(ref obj.playerName, writer);
    }

    public static void NetDeserialize(ref NetMessageClientHello obj, BitStreamReader reader)
    {
        NetSerializer_String.NetDeserialize(ref obj.playerName, reader);
    }
}
