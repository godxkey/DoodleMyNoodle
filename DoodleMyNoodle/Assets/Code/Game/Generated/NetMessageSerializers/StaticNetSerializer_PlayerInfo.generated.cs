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
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.IsMaster);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
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
        StaticNetSerializer_System_String.NetSerialize(ref obj.PlayerName, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.IsMaster, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
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
        StaticNetSerializer_System_String.NetDeserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.IsMaster, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
    }
}
