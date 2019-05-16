// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_FixMatrix
{
    public static int GetNetBitSize(ref FixMatrix obj)
    {
        int result = 0;
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M13);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M14);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M23);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M24);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M31);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M32);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M33);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M34);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M41);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M42);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M43);
        result += StaticNetSerializer_Fix64.GetNetBitSize(ref obj.M44);
        return result;
    }

    public static void NetSerialize(ref FixMatrix obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M13, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M14, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M23, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M24, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M31, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M32, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M33, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M34, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M41, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M42, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M43, writer);
        StaticNetSerializer_Fix64.NetSerialize(ref obj.M44, writer);
    }

    public static void NetDeserialize(ref FixMatrix obj, BitStreamReader reader)
    {
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M13, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M14, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M23, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M24, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M31, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M32, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M33, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M34, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M41, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M42, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M43, reader);
        StaticNetSerializer_Fix64.NetDeserialize(ref obj.M44, reader);
    }
}
