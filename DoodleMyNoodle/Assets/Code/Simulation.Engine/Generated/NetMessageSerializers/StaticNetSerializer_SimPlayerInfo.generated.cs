// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimPlayerInfo
{
    public static int GetNetBitSize_Class(SimPlayerInfo obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInfo obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.Name);
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.SimPlayerId);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInfo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInfo obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.Name, writer);
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.SimPlayerId, writer);
    }

    public static SimPlayerInfo NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInfo obj = new SimPlayerInfo();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInfo obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.Name, reader);
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.SimPlayerId, reader);
    }
}
