// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixVector3
{
    public static int GetNetBitSize(ref FixVector3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.z);
        return result;
    }

    public static void NetSerialize(ref FixVector3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.z, writer);
    }

    public static void NetDeserialize(ref FixVector3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.z, reader);
    }
}
