// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fix2
{
    public static int GetNetBitSize(ref fix2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        return result;
    }

    public static void NetSerialize(ref fix2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
    }

    public static void NetDeserialize(ref fix2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
    }
}
