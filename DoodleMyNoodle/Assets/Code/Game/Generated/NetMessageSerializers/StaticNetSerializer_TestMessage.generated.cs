// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_TestMessage
{
    public static int GetNetBitSize_Class(TestMessage obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.valueString);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.valueInt);
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.valueUInt);
        result += StaticNetSerializer_System_Int16.GetNetBitSize(ref obj.valueShort);
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.valueUShort);
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.valueBool);
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_System_Int32.GetNetBitSize(ref obj.listOnInts);
        result += StaticNetSerializer_TestMessageCat.GetNetBitSize_Class(obj.cat);
        result += StaticNetSerializer_TestMessageDog.GetNetBitSize_Class(obj.dog);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize_Class(obj.animal);
        result += ArrayNetSerializer_TestMessageAnimal.GetNetBitSize(ref obj.animals);
        return result;
    }

    public static void NetSerialize_Class(TestMessage obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.NetSerialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.NetSerialize(ref obj.listOnInts, writer);
        StaticNetSerializer_TestMessageCat.NetSerialize_Class(obj.cat, writer);
        StaticNetSerializer_TestMessageDog.NetSerialize_Class(obj.dog, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize_Class(obj.animal, writer);
        ArrayNetSerializer_TestMessageAnimal.NetSerialize(ref obj.animals, writer);
    }

    public static TestMessage NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessage obj = new TestMessage();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.NetDeserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.NetDeserialize(ref obj.listOnInts, reader);
        obj.cat = StaticNetSerializer_TestMessageCat.NetDeserialize_Class(reader);
        obj.dog = StaticNetSerializer_TestMessageDog.NetDeserialize_Class(reader);
        obj.animal = StaticNetSerializer_TestMessageAnimal.NetDeserialize_Class(reader);
        ArrayNetSerializer_TestMessageAnimal.NetDeserialize(ref obj.animals, reader);
    }
}
