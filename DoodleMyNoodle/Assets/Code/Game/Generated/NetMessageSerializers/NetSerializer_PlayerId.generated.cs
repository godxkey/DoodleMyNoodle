// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_PlayerId
{
    public static int GetNetBitSize(ref PlayerId obj)
    {
        int result = 0;
        result += NetSerializer_UInt16.GetNetBitSize(ref obj.value);
        return result;
    }

    public static void NetSerialize(ref PlayerId obj, BitStreamWriter writer)
    {
        NetSerializer_UInt16.NetSerialize(ref obj.value, writer);
    }

    public static void NetDeserialize(ref PlayerId obj, BitStreamReader reader)
    {
        NetSerializer_UInt16.NetDeserialize(ref obj.value, reader);
    }
}
