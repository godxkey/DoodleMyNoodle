// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputLog
{
    public static int GetNetBitSize_Class(SimInputLog obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputLog obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.message);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputLog obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputLog obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.message, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputLog NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputLog obj = new SimInputLog();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputLog obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.message, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
