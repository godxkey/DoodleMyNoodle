// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSerializedSimulation
{
    public static int GetNetBitSize(ref NetMessageSerializedSimulation obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.SerializedSimulation);
        return result;
    }

    public static void NetSerialize(ref NetMessageSerializedSimulation obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.SerializedSimulation, writer);
    }

    public static void NetDeserialize(ref NetMessageSerializedSimulation obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.SerializedSimulation, reader);
    }
}
