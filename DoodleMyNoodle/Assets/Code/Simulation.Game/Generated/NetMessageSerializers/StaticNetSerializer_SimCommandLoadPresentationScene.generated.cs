// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommandLoadPresentationScene
{
    public static int GetNetBitSize_Class(SimCommandLoadPresentationScene obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandLoadPresentationScene obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.SceneName);
        result += StaticNetSerializer_SimCommand.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandLoadPresentationScene obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandLoadPresentationScene obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.SceneName, writer);
        StaticNetSerializer_SimCommand.NetSerialize(obj, writer);
    }

    public static SimCommandLoadPresentationScene NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandLoadPresentationScene obj = new SimCommandLoadPresentationScene();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandLoadPresentationScene obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.SceneName, reader);
        StaticNetSerializer_SimCommand.NetDeserialize(obj, reader);
    }
}
