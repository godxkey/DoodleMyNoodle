// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_PlayerInfo
{
    public static int GetNetBitSize_Class(PlayerInfo obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(PlayerInfo obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.playerName);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        result += StaticNetSerializer_Boolean.GetNetBitSize(ref obj.isServer);
        return result;
    }

    public static void NetSerialize_Class(PlayerInfo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(PlayerInfo obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.playerName, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
        StaticNetSerializer_Boolean.NetSerialize(ref obj.isServer, writer);
    }

    public static PlayerInfo NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        PlayerInfo obj = new PlayerInfo();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(PlayerInfo obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.playerName, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
        StaticNetSerializer_Boolean.NetDeserialize(ref obj.isServer, reader);
    }
}
