// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputPlayerUpdate
{
    public static int GetNetBitSize_Class(SimInputPlayerUpdate obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputPlayerUpdate obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimPlayerInfo.GetNetBitSize_Class(obj.PlayerInfo);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputPlayerUpdate obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputPlayerUpdate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimPlayerInfo.NetSerialize_Class(obj.PlayerInfo, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputPlayerUpdate NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputPlayerUpdate obj = new SimInputPlayerUpdate();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputPlayerUpdate obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.PlayerId, reader);
        obj.PlayerInfo = StaticNetSerializer_SimPlayerInfo.NetDeserialize_Class(reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
