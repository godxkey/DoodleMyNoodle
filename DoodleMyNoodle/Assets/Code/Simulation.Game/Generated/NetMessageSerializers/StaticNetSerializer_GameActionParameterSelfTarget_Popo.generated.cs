// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameActionParameterSelfTarget_Popo
{
    public static int GetNetBitSize_Class(GameActionParameterSelfTarget.Popo obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameActionParameterSelfTarget.Popo obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.ParamIndex);
        result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(GameActionParameterSelfTarget.Popo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameActionParameterSelfTarget.Popo obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.ParamIndex, writer);
        StaticNetSerializer_GameAction_ParameterData.NetSerialize(obj, writer);
    }

    public static GameActionParameterSelfTarget.Popo NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterSelfTarget.Popo obj = new GameActionParameterSelfTarget.Popo();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameActionParameterSelfTarget.Popo obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.ParamIndex, reader);
        StaticNetSerializer_GameAction_ParameterData.NetDeserialize(obj, reader);
    }
}
