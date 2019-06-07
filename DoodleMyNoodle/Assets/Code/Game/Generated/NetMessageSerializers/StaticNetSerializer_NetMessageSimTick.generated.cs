// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSimTick
{
    public static int GetNetBitSize(ref NetMessageSimTick obj)
    {
        int result = 0;
        result += StaticNetSerializer_UInt32.GetNetBitSize(ref obj.tickId);
        result += ArrayNetSerializer_ApprovedSimInput.GetNetBitSize(ref obj.inputs);
        return result;
    }

    public static void NetSerialize(ref NetMessageSimTick obj, BitStreamWriter writer)
    {
        StaticNetSerializer_UInt32.NetSerialize(ref obj.tickId, writer);
        ArrayNetSerializer_ApprovedSimInput.NetSerialize(ref obj.inputs, writer);
    }

    public static void NetDeserialize(ref NetMessageSimTick obj, BitStreamReader reader)
    {
        StaticNetSerializer_UInt32.NetDeserialize(ref obj.tickId, reader);
        ArrayNetSerializer_ApprovedSimInput.NetDeserialize(ref obj.inputs, reader);
    }
}
