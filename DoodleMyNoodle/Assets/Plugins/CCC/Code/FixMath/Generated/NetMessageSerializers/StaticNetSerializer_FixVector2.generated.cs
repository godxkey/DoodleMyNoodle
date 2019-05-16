// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixVector2
{
    public static int GetNetBitSize(ref FixVector2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.X);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.Y);
        return result;
    }

    public static void NetSerialize(ref FixVector2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.X, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.Y, writer);
    }

    public static void NetDeserialize(ref FixVector2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.X, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.Y, reader);
    }
}
