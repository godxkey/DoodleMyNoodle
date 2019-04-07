// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_Popo
{
    public static int GetNetBitSize(ref Popo obj)
    {
        int result = 0;
        result += NetSerializer_Single.GetNetBitSize(ref obj.someFloat);
        result += NetSerializer_Int32.GetNetBitSize(ref obj.someInt);
        return result;
    }

    public static void NetSerialize(ref Popo obj, BitStreamWriter writer)
    {
        NetSerializer_Single.NetSerialize(ref obj.someFloat, writer);
        NetSerializer_Int32.NetSerialize(ref obj.someInt, writer);
    }

    public static void NetDeserialize(ref Popo obj, BitStreamReader reader)
    {
        NetSerializer_Single.NetDeserialize(ref obj.someFloat, reader);
        NetSerializer_Int32.NetDeserialize(ref obj.someInt, reader);
    }
}
