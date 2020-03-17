// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_TestMessageAnimal
{
    public static int GetNetBitSize_Class(TestMessageAnimal obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageAnimal obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        return result;
    }

    public static void NetSerialize_Class(TestMessageAnimal obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageAnimal obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
    }

    public static TestMessageAnimal NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (TestMessageAnimal)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(TestMessageAnimal obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
    }
}
