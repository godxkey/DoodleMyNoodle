// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimPlayerId
{
    public static int GetNetBitSize(ref SimPlayerId obj)
    {
        int result = 0;
        result += StaticNetSerializer_UInt32.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref SimPlayerId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_UInt32.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref SimPlayerId obj, BitStreamReader reader)
    {
        StaticNetSerializer_UInt32.NetDeserialize(ref obj.Value, reader);
    }
}
