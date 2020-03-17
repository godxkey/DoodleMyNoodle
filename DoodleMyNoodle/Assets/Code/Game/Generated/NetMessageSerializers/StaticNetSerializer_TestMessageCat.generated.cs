// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_TestMessageCat
{
    public static int GetNetBitSize_Class(TestMessageCat obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageCat obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.numberOfLivesLeft);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(TestMessageCat obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageCat obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.numberOfLivesLeft, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize(obj, writer);
    }

    public static TestMessageCat NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageCat obj = new TestMessageCat();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessageCat obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.numberOfLivesLeft, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
    }
}
