// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimTileId_OLD
{
    public static int GetNetBitSize(ref SimTileId_OLD obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.X);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.Y);
        return result;
    }

    public static void NetSerialize(ref SimTileId_OLD obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.X, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.Y, writer);
    }

    public static void NetDeserialize(ref SimTileId_OLD obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.X, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.Y, reader);
    }
}
