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
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_Boolean.GetNetBitSize(ref obj.IsServer);
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.SimPlayerId);
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
        StaticNetSerializer_String.NetSerialize(ref obj.PlayerName, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_Boolean.NetSerialize(ref obj.IsServer, writer);
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.SimPlayerId, writer);
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
        StaticNetSerializer_String.NetDeserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_Boolean.NetDeserialize(ref obj.IsServer, reader);
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.SimPlayerId, reader);
    }
}
