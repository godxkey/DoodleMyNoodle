// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_SimInputSubmission
{
    public static int GetNetBitSize(ref SimInputSubmission[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_SimInputSubmission.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref SimInputSubmission[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_SimInputSubmission.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref SimInputSubmission[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new SimInputSubmission[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_SimInputSubmission.NetDeserialize(ref obj[i], reader);
        }
    }
}
