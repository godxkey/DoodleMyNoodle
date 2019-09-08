// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommandLog
{
    public static int GetNetBitSize_Class(SimCommandLog obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandLog obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.message);
        result += StaticNetSerializer_SimCommand.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandLog obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandLog obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.message, writer);
        StaticNetSerializer_SimCommand.NetSerialize(obj, writer);
    }

    public static SimCommandLog NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandLog obj = new SimCommandLog();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandLog obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.message, reader);
        StaticNetSerializer_SimCommand.NetDeserialize(obj, reader);
    }
}
