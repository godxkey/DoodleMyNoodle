// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSimSyncFromFile
{
    public static int GetNetBitSize(ref NetMessageSimSyncFromFile obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.SerializedSimulationFilePath);
        return result;
    }

    public static void NetSerialize(ref NetMessageSimSyncFromFile obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.SerializedSimulationFilePath, writer);
    }

    public static void NetDeserialize(ref NetMessageSimSyncFromFile obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.SerializedSimulationFilePath, reader);
    }
}
