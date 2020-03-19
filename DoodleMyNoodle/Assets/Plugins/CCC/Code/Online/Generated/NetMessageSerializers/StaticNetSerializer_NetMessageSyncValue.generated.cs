// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSyncValue
{
    public static int GetNetBitSize(ref NetMessageSyncValue obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.ValueData);
        return result;
    }

    public static void NetSerialize(ref NetMessageSyncValue obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.ValueData, writer);
    }

    public static void NetDeserialize(ref NetMessageSyncValue obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.ValueData, reader);
    }
}
