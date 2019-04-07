// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_PlayerInfo
{
    public static int GetNetBitSize(ref PlayerInfo obj)
    {
        if (obj == null)
            return 1;
        int result = 1;
        result += NetSerializer_String.GetNetBitSize(ref obj.playerName);
        result += NetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        result += NetSerializer_Boolean.GetNetBitSize(ref obj.isServer);
        return result;
    }

    public static void NetSerialize(ref PlayerInfo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer_String.NetSerialize(ref obj.playerName, writer);
        NetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
        NetSerializer_Boolean.NetSerialize(ref obj.isServer, writer);
    }

    public static void NetDeserialize(ref PlayerInfo obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new PlayerInfo();
        NetSerializer_String.NetDeserialize(ref obj.playerName, reader);
        NetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
        NetSerializer_Boolean.NetDeserialize(ref obj.isServer, reader);
    }
}
