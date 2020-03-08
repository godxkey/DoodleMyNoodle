// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimTileId
{
    public static int GetNetBitSize(ref SimTileId obj)
    {
        int result = 0;
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.X);
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.Y);
        return result;
    }

    public static void NetSerialize(ref SimTileId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Int32.NetSerialize(ref obj.X, writer);
        StaticNetSerializer_Int32.NetSerialize(ref obj.Y, writer);
    }

    public static void NetDeserialize(ref SimTileId obj, BitStreamReader reader)
    {
        StaticNetSerializer_Int32.NetDeserialize(ref obj.X, reader);
        StaticNetSerializer_Int32.NetDeserialize(ref obj.Y, reader);
    }
}
