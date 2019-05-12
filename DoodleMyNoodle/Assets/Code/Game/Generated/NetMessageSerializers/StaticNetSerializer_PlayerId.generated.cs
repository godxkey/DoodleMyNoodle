// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_PlayerId
{
    public static int GetNetBitSize(ref PlayerId obj)
    {
        int result = 0;
        result += StaticNetSerializer_UInt16.GetNetBitSize(ref obj.value);
        return result;
    }

    public static void NetSerialize(ref PlayerId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_UInt16.NetSerialize(ref obj.value, writer);
    }

    public static void NetDeserialize(ref PlayerId obj, BitStreamReader reader)
    {
        StaticNetSerializer_UInt16.NetDeserialize(ref obj.value, reader);
    }
}
