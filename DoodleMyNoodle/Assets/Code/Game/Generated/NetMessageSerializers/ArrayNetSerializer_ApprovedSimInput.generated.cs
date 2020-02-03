// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_ApprovedSimInput
{
    public static int GetNetBitSize(ref ApprovedSimInput[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_ApprovedSimInput.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref ApprovedSimInput[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteUInt32((UInt32)obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_ApprovedSimInput.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref ApprovedSimInput[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new ApprovedSimInput[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_ApprovedSimInput.NetDeserialize(ref obj[i], reader);
        }
    }
}
