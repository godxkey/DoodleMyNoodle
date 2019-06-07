// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputInstantiate
{
    public static int GetNetBitSize_Class(SimInputInstantiate obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputInstantiate obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimBlueprintId.GetNetBitSize(ref obj.blueprintId);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputInstantiate obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputInstantiate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimBlueprintId.NetSerialize(ref obj.blueprintId, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputInstantiate NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputInstantiate obj = new SimInputInstantiate();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputInstantiate obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimBlueprintId.NetDeserialize(ref obj.blueprintId, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
