// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputKeycode
{
    public static int GetNetBitSize_Class(SimInputKeycode obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputKeycode obj)
    {
        int result = 0;
        result += StaticNetSerializer_Int32.GetNetBitSize();
        result += StaticNetSerializer_Int32.GetNetBitSize();
        result += StaticNetSerializer_SimPlayerId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputKeycode obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputKeycode obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Int32.NetSerialize((System.Int32)obj.state, writer);
        StaticNetSerializer_Int32.NetSerialize((System.Int32)obj.keyCode, writer);
        StaticNetSerializer_SimPlayerId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimInputKeycode NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputKeycode obj = new SimInputKeycode();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputKeycode obj, BitStreamReader reader)
    {
        obj.state = (SimInputKeycode.State)StaticNetSerializer_Int32.NetDeserialize(reader);
        obj.keyCode = (UnityEngine.KeyCode)StaticNetSerializer_Int32.NetDeserialize(reader);
        StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
