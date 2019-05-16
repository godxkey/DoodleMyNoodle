// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixVector4
{
    public static int GetNetBitSize(ref FixVector4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.X);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.Y);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.Z);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.W);
        return result;
    }

    public static void NetSerialize(ref FixVector4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.X, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.Y, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.Z, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.W, writer);
    }

    public static void NetDeserialize(ref FixVector4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.X, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.Y, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.Z, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.W, reader);
    }
}
