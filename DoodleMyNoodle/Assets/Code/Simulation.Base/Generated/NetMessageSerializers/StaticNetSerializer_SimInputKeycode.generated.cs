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
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
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
        StaticNetSerializer_Int32.NetSerialize((System.Int32)obj.keyCode, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
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
        obj.keyCode = (UnityEngine.KeyCode)StaticNetSerializer_Int32.NetDeserialize(reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
