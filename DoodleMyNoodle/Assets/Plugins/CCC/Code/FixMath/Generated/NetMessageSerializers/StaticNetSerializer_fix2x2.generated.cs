// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fix2x2
{
    public static int GetNetBitSize(ref fix2x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        return result;
    }

    public static void NetSerialize(ref fix2x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
    }

    public static void NetDeserialize(ref fix2x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
    }
}
