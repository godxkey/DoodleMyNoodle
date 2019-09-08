// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputPlayerRemove
{
    public static int GetNetBitSize_Class(SimInputPlayerRemove obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputPlayerRemove obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputPlayerRemove obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputPlayerRemove obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputPlayerRemove NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputPlayerRemove obj = new SimInputPlayerRemove();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputPlayerRemove obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
