// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageDestroyValue
{
    public static int GetNetBitSize(ref NetMessageDestroyValue obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TypeId);
        return result;
    }

    public static void NetSerialize(ref NetMessageDestroyValue obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TypeId, writer);
    }

    public static void NetDeserialize(ref NetMessageDestroyValue obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TypeId, reader);
    }
}
