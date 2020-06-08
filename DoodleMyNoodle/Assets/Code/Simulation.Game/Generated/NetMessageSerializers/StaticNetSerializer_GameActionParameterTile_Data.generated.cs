// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameActionParameterTile_Data
{
    public static int GetNetBitSize_Class(GameActionParameterTile.Data obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameActionParameterTile.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetNetBitSize(ref obj.Tile);
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.ParamIndex);
        result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetSerialize(ref obj.Tile, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.ParamIndex, writer);
        StaticNetSerializer_GameAction_ParameterData.NetSerialize(obj, writer);
    }

    public static GameActionParameterTile.Data NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterTile.Data obj = new GameActionParameterTile.Data();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameActionParameterTile.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetDeserialize(ref obj.Tile, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.ParamIndex, reader);
        StaticNetSerializer_GameAction_ParameterData.NetDeserialize(obj, reader);
    }
}
