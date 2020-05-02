// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameAction_ParameterData
{
    public static int GetNetBitSize_Class(GameAction.ParameterData obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameAction.ParameterData obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.ParamIndex);
        return result;
    }

    public static void NetSerialize_Class(GameAction.ParameterData obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameAction.ParameterData obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.ParamIndex, writer);
    }

    public static GameAction.ParameterData NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (GameAction.ParameterData)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(GameAction.ParameterData obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.ParamIndex, reader);
    }
}
