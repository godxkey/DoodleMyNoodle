// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommandMoveBall
{
    public static int GetNetBitSize_Class(SimCommandMoveBall obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandMoveBall obj)
    {
        int result = 0;
        result += StaticNetSerializer_FixVector3.GetNetBitSize(ref obj.moveDirection);
        result += StaticNetSerializer_SimCommand.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandMoveBall obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandMoveBall obj, BitStreamWriter writer)
    {
        StaticNetSerializer_FixVector3.NetSerialize(ref obj.moveDirection, writer);
        StaticNetSerializer_SimCommand.NetSerialize(obj, writer);
    }

    public static SimCommandMoveBall NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandMoveBall obj = new SimCommandMoveBall();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandMoveBall obj, BitStreamReader reader)
    {
        StaticNetSerializer_FixVector3.NetDeserialize(ref obj.moveDirection, reader);
        StaticNetSerializer_SimCommand.NetDeserialize(obj, reader);
    }
}
