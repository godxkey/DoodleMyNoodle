// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputMoveBall
{
    public static int GetNetBitSize_Class(SimInputMoveBall obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputMoveBall obj)
    {
        int result = 0;
        result += StaticNetSerializer_FixVector2.GetNetBitSize(ref obj.moveDirection);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputMoveBall obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputMoveBall obj, BitStreamWriter writer)
    {
        StaticNetSerializer_FixVector2.NetSerialize(ref obj.moveDirection, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimInputMoveBall NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputMoveBall obj = new SimInputMoveBall();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputMoveBall obj, BitStreamReader reader)
    {
        StaticNetSerializer_FixVector2.NetDeserialize(ref obj.moveDirection, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
