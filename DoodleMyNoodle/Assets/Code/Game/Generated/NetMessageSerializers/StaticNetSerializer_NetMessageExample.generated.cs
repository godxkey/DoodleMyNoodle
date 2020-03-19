// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageExample
{
    public static int GetNetBitSize_Class(NetMessageExample obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(NetMessageExample obj)
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
        return result;
    }

    public static void NetSerialize_Class(NetMessageExample obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(NetMessageExample obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.NetSerialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.NetSerialize(ref obj.listOnInts, writer);
    }

    public static NetMessageExample NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        NetMessageExample obj = new NetMessageExample();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(NetMessageExample obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.NetDeserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.NetDeserialize(ref obj.listOnInts, reader);
    }
}
