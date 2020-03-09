// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimPlayerInputUseItem
{
    public static int GetNetBitSize_Class(SimPlayerInputUseItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputUseItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.ItemIndex);
        result += ArrayNetSerializer_Object.GetNetBitSize(ref obj.Informations);
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Int32.NetSerialize(ref obj.ItemIndex, writer);
        ArrayNetSerializer_Object.NetSerialize(ref obj.Informations, writer);
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputUseItem NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputUseItem obj = new SimPlayerInputUseItem();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputUseItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_Int32.NetDeserialize(ref obj.ItemIndex, reader);
        ArrayNetSerializer_Object.NetDeserialize(ref obj.Informations, reader);
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
