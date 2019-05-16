// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixMatrix2x2
{
    public static int GetNetBitSize(ref FixMatrix2x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M22);
        return result;
    }

    public static void NetSerialize(ref FixMatrix2x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M22, writer);
    }

    public static void NetDeserialize(ref FixMatrix2x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M22, reader);
    }
}
