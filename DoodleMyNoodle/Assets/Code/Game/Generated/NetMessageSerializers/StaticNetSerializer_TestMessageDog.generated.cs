// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_TestMessageDog
{
    public static int GetNetBitSize_Class(TestMessageDog obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageDog obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.isAGoodBoy);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(TestMessageDog obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageDog obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.isAGoodBoy, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize(obj, writer);
    }

    public static TestMessageDog NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageDog obj = new TestMessageDog();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessageDog obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.isAGoodBoy, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
    }
}
