// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameAction_UseParameters
{
    public static int GetNetBitSize_Class(GameAction.UseParameters obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameAction.UseParameters obj)
    {
        int result = 0;
        result += ArrayNetSerializer_GameAction_ParameterData.GetNetBitSize(ref obj.ParameterDatas);
        return result;
    }

    public static void NetSerialize_Class(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetSerialize(ref obj.ParameterDatas, writer);
    }

    public static GameAction.UseParameters NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameAction.UseParameters obj = new GameAction.UseParameters();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameAction.UseParameters obj, BitStreamReader reader)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetDeserialize(ref obj.ParameterDatas, reader);
    }
}
