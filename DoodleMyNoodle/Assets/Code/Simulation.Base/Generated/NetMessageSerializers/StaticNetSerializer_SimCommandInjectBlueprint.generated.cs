// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommandInjectBlueprint
{
    public static int GetNetBitSize_Class(SimCommandInjectBlueprint obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandInjectBlueprint obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimBlueprintId.GetNetBitSize(ref obj.blueprintId);
        result += StaticNetSerializer_SimCommand.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandInjectBlueprint obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandInjectBlueprint obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimBlueprintId.NetSerialize(ref obj.blueprintId, writer);
        StaticNetSerializer_SimCommand.NetSerialize(obj, writer);
    }

    public static SimCommandInjectBlueprint NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandInjectBlueprint obj = new SimCommandInjectBlueprint();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandInjectBlueprint obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimBlueprintId.NetDeserialize(ref obj.blueprintId, reader);
        StaticNetSerializer_SimCommand.NetDeserialize(obj, reader);
    }
}
