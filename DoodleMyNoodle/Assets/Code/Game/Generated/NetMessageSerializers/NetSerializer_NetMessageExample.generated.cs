// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessageExample
{
    public static int GetNetBitSize(ref NetMessageExample obj)
    {
        if (obj == null)
            return 1;
        int result = 1;
        result += NetSerializer_String.GetNetBitSize(ref obj.valueString);
        result += NetSerializer_Int32.GetNetBitSize(ref obj.valueInt);
        result += NetSerializer_UInt32.GetNetBitSize(ref obj.valueUInt);
        result += NetSerializer_Int16.GetNetBitSize(ref obj.valueShort);
        result += NetSerializer_UInt16.GetNetBitSize(ref obj.valueUShort);
        result += NetSerializer_Boolean.GetNetBitSize(ref obj.valueBool);
        result += NetSerializer_Byte.GetNetBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_Int32.GetNetBitSize(ref obj.listOnInts);
        return result;
    }

    public static void NetSerialize(ref NetMessageExample obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer_String.NetSerialize(ref obj.valueString, writer);
        NetSerializer_Int32.NetSerialize(ref obj.valueInt, writer);
        NetSerializer_UInt32.NetSerialize(ref obj.valueUInt, writer);
        NetSerializer_Int16.NetSerialize(ref obj.valueShort, writer);
        NetSerializer_UInt16.NetSerialize(ref obj.valueUShort, writer);
        NetSerializer_Boolean.NetSerialize(ref obj.valueBool, writer);
        NetSerializer_Byte.NetSerialize(ref obj.valueByte, writer);
        ArrayNetSerializer_Int32.NetSerialize(ref obj.listOnInts, writer);
    }

    public static void NetDeserialize(ref NetMessageExample obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new NetMessageExample();
        NetSerializer_String.NetDeserialize(ref obj.valueString, reader);
        NetSerializer_Int32.NetDeserialize(ref obj.valueInt, reader);
        NetSerializer_UInt32.NetDeserialize(ref obj.valueUInt, reader);
        NetSerializer_Int16.NetDeserialize(ref obj.valueShort, reader);
        NetSerializer_UInt16.NetDeserialize(ref obj.valueUShort, reader);
        NetSerializer_Boolean.NetDeserialize(ref obj.valueBool, reader);
        NetSerializer_Byte.NetDeserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_Int32.NetDeserialize(ref obj.listOnInts, reader);
    }
}
