// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixVector4
{
    public static int GetNetBitSize(ref FixVector4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.z);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.w);
        return result;
    }

    public static void NetSerialize(ref FixVector4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.z, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.w, writer);
    }

    public static void NetDeserialize(ref FixVector4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.z, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.w, reader);
    }
}
