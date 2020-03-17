// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimMasterInput
{
    public static int GetNetBitSize_Class(SimMasterInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimMasterInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimMasterInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimMasterInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimMasterInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimMasterInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimMasterInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
