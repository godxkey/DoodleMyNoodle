// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fixQuaternion
{
    public static int GetNetBitSize(ref fixQuaternion obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.z);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.w);
        return result;
    }

    public static void NetSerialize(ref fixQuaternion obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.z, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.w, writer);
    }

    public static void NetDeserialize(ref fixQuaternion obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.z, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.w, reader);
    }
}
