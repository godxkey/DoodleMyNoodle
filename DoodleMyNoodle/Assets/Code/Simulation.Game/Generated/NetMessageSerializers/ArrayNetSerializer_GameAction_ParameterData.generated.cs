// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_GameAction_ParameterData
{
    public static int GetNetBitSize(ref GameAction.ParameterData[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref GameAction.ParameterData[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteUInt32((UInt32)obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_GameAction_ParameterData.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref GameAction.ParameterData[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new GameAction.ParameterData[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_GameAction_ParameterData.NetDeserialize_Class(reader);
        }
    }
}
