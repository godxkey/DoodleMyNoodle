// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_PersistentId
{
    public static int GetNetBitSize(ref PersistentId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref PersistentId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref PersistentId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.Value, reader);
    }
}
