// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_TestMessageAnimal
{
    public static int GetNetBitSize(ref TestMessageAnimal[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref TestMessageAnimal[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_TestMessageAnimal.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref TestMessageAnimal[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new TestMessageAnimal[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_TestMessageAnimal.NetDeserialize_Class(reader);
        }
    }
}
