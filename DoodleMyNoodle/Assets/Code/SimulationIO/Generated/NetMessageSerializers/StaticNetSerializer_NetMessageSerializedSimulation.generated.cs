// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSerializedSimulation
{
    public static int GetNetBitSize(ref NetMessageSerializedSimulation obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.SerializedSimulation);
        return result;
    }

    public static void NetSerialize(ref NetMessageSerializedSimulation obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.SerializedSimulation, writer);
    }

    public static void NetDeserialize(ref NetMessageSerializedSimulation obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.SerializedSimulation, reader);
    }
}
