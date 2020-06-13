// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimPlayerInput
{
    public static int GetNetBitSize_Class(SimPlayerInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInput obj = new SimPlayerInput();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
