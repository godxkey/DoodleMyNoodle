// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInput
{
    public static int GetNetBitSize_Class(SimInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInput obj)
    {
        int result = 0;
        return result;
    }

    public static void NetSerialize_Class(SimInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInput obj, BitStreamWriter writer)
    {
    }

    public static SimInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimInput obj, BitStreamReader reader)
    {
    }
}
