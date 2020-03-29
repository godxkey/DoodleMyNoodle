// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fix2x3
{
    public static int GetNetBitSize(ref fix2x3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M23);
        return result;
    }

    public static void NetSerialize(ref fix2x3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M13, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M23, writer);
    }

    public static void NetDeserialize(ref fix2x3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M23, reader);
    }
}
