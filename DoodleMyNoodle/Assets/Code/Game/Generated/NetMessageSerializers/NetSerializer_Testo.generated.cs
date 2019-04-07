// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_Testo
{
    public static int GetNetBitSize(ref Testo obj)
    {
        if (obj == null)
            return 1;
        int result = 1;
        result += NetSerializer_Popo.GetNetBitSize(ref obj.somePopo);
        result += NetSerializer_String.GetNetBitSize(ref obj.someString);
        return result;
    }

    public static void NetSerialize(ref Testo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer_Popo.NetSerialize(ref obj.somePopo, writer);
        NetSerializer_String.NetSerialize(ref obj.someString, writer);
    }

    public static void NetDeserialize(ref Testo obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new Testo();
        NetSerializer_Popo.NetDeserialize(ref obj.somePopo, reader);
        NetSerializer_String.NetDeserialize(ref obj.someString, reader);
    }
}
