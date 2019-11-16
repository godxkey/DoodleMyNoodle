// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_Fix64
{
    public static int GetNetBitSize(ref Fix64 obj)
    {
        int result = 0;
        result += StaticNetSerializer_Int64.GetNetBitSize(ref obj.RawValue);
        return result;
    }

    public static void NetSerialize(ref Fix64 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Int64.NetSerialize(ref obj.RawValue, writer);
    }

    public static void NetDeserialize(ref Fix64 obj, BitStreamReader reader)
    {
        StaticNetSerializer_Int64.NetDeserialize(ref obj.RawValue, reader);
    }
}
