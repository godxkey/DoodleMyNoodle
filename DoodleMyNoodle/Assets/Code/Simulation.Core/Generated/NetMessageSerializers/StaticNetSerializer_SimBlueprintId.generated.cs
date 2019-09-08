// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimBlueprintId
{
    public static int GetNetBitSize(ref SimBlueprintId obj)
    {
        int result = 0;
        result += StaticNetSerializer_Byte.GetNetBitSize();
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref SimBlueprintId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Byte.NetSerialize((System.Byte)obj.Type, writer);
        StaticNetSerializer_String.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref SimBlueprintId obj, BitStreamReader reader)
    {
        obj.Type = (SimBlueprintId.BlueprintType)StaticNetSerializer_Byte.NetDeserialize(reader);
        StaticNetSerializer_String.NetDeserialize(ref obj.Value, reader);
    }
}
