// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageSimPlayerIdAssignement
{
    public static int GetNetBitSize(ref NetMessageSimPlayerIdAssignement obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.PlayerId);
        return result;
    }

    public static void NetSerialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.PlayerId, writer);
    }

    public static void NetDeserialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.PlayerId, reader);
    }
}
