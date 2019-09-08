// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputPlayerCreate
{
    public static int GetNetBitSize_Class(SimInputPlayerCreate obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputPlayerCreate obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimPlayerInfo.GetNetBitSize_Class(obj.SimPlayerInfo);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimPlayerInfo.NetSerialize_Class(obj.SimPlayerInfo, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputPlayerCreate NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputPlayerCreate obj = new SimInputPlayerCreate();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputPlayerCreate obj, BitStreamReader reader)
    {
        obj.SimPlayerInfo = StaticNetSerializer_SimPlayerInfo.NetDeserialize_Class(reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
