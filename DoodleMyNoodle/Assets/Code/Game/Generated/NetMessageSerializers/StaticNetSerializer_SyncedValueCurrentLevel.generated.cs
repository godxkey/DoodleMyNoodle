// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SyncedValueCurrentLevel
{
    public static int GetNetBitSize(ref SyncedValueCurrentLevel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Name);
        return result;
    }

    public static void NetSerialize(ref SyncedValueCurrentLevel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.Name, writer);
    }

    public static void NetDeserialize(ref SyncedValueCurrentLevel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Name, reader);
    }
}
