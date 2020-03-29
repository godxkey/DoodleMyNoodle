// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fix
{
    public static int GetNetBitSize(ref fix obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int64.GetNetBitSize(ref obj.RawValue);
        return result;
    }

    public static void NetSerialize(ref fix obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int64.NetSerialize(ref obj.RawValue, writer);
    }

    public static void NetDeserialize(ref fix obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int64.NetDeserialize(ref obj.RawValue, reader);
    }
}
