// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommand
{
    public static int GetNetBitSize_Class(SimCommand obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommand obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommand obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommand obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimCommand NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimCommand)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimCommand obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
