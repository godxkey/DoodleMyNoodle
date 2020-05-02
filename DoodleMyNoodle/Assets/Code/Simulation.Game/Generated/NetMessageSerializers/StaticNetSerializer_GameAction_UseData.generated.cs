// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameAction_UseData
{
    public static int GetNetBitSize_Class(GameAction.UseData obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameAction.UseData obj)
    {
        int result = 0;
        result += ArrayNetSerializer_GameAction_ParameterData.GetNetBitSize(ref obj.ParameterDatas);
        return result;
    }

    public static void NetSerialize_Class(GameAction.UseData obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameAction.UseData obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetSerialize(ref obj.ParameterDatas, writer);
    }

    public static GameAction.UseData NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameAction.UseData obj = new GameAction.UseData();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameAction.UseData obj, BitStreamReader reader)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetDeserialize(ref obj.ParameterDatas, reader);
    }
}
