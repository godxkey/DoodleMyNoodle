// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimBlueprintId
{
    public static int GetNetBitSize(ref SimBlueprintId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Byte.GetNetBitSize();
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref SimBlueprintId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Byte.NetSerialize((System.Byte)obj.Type, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref SimBlueprintId obj, BitStreamReader reader)
    {
        obj.Type = (SimBlueprintId.BlueprintType)StaticNetSerializer_System_Byte.NetDeserialize(reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Value, reader);
    }
}
