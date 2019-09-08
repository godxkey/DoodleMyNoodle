// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimCommandLoadScene
{
    public static int GetNetBitSize_Class(SimCommandLoadScene obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandLoadScene obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.sceneName);
        result += StaticNetSerializer_SimCommand.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.sceneName, writer);
        StaticNetSerializer_SimCommand.NetSerialize(obj, writer);
    }

    public static SimCommandLoadScene NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandLoadScene obj = new SimCommandLoadScene();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandLoadScene obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.sceneName, reader);
        StaticNetSerializer_SimCommand.NetDeserialize(obj, reader);
    }
}
