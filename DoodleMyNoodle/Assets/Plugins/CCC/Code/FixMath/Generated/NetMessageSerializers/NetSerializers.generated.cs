// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_fix
{
    public static int GetNetBitSize(ref fix obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int64.GetNetBitSize(ref obj.RawValue);
        return result;
    }

    public static void NetSerialize(ref fix obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int64.NetSerialize(ref obj.RawValue, writer);
    }

    public static void NetDeserialize(ref fix obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int64.NetDeserialize(ref obj.RawValue, reader);
    }
}
public static class StaticNetSerializer_fix2
{
    public static int GetNetBitSize(ref fix2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        return result;
    }

    public static void NetSerialize(ref fix2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
    }

    public static void NetDeserialize(ref fix2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
    }
}
public static class StaticNetSerializer_fix2x2
{
    public static int GetNetBitSize(ref fix2x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        return result;
    }

    public static void NetSerialize(ref fix2x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
    }

    public static void NetDeserialize(ref fix2x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
    }
}
public static class StaticNetSerializer_fix2x3
{
    public static int GetNetBitSize(ref fix2x3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M23);
        return result;
    }

    public static void NetSerialize(ref fix2x3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M13, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M23, writer);
    }

    public static void NetDeserialize(ref fix2x3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M23, reader);
    }
}
public static class StaticNetSerializer_fix3
{
    public static int GetNetBitSize(ref fix3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.z);
        return result;
    }

    public static void NetSerialize(ref fix3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.z, writer);
    }

    public static void NetDeserialize(ref fix3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.z, reader);
    }
}
public static class StaticNetSerializer_fix3x2
{
    public static int GetNetBitSize(ref fix3x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M32);
        return result;
    }

    public static void NetSerialize(ref fix3x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M31, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M32, writer);
    }

    public static void NetDeserialize(ref fix3x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M32, reader);
    }
}
public static class StaticNetSerializer_fix3x3
{
    public static int GetNetBitSize(ref fix3x3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M23);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M32);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M33);
        return result;
    }

    public static void NetSerialize(ref fix3x3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M13, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M23, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M31, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M32, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M33, writer);
    }

    public static void NetDeserialize(ref fix3x3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M23, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M32, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M33, reader);
    }
}
public static class StaticNetSerializer_fix4
{
    public static int GetNetBitSize(ref fix4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.z);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.w);
        return result;
    }

    public static void NetSerialize(ref fix4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.z, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.w, writer);
    }

    public static void NetDeserialize(ref fix4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.z, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.w, reader);
    }
}
public static class StaticNetSerializer_fix4x4
{
    public static int GetNetBitSize(ref fix4x4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M41);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M32);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M42);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M23);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M33);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M43);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M14);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M24);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M34);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.M44);
        return result;
    }

    public static void NetSerialize(ref fix4x4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.M11, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M21, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M31, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M41, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M12, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M22, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M32, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M42, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M13, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M23, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M33, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M43, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M14, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M24, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M34, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.M44, writer);
    }

    public static void NetDeserialize(ref fix4x4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M41, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M32, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M42, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M23, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M33, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M43, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M14, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M24, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M34, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.M44, reader);
    }
}
public static class StaticNetSerializer_fixQuaternion
{
    public static int GetNetBitSize(ref fixQuaternion obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.z);
        result += StaticNetSerializer_fix.GetNetBitSize(ref obj.w);
        return result;
    }

    public static void NetSerialize(ref fixQuaternion obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.NetSerialize(ref obj.x, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.y, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.z, writer);
        StaticNetSerializer_fix.NetSerialize(ref obj.w, writer);
    }

    public static void NetDeserialize(ref fixQuaternion obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.NetDeserialize(ref obj.x, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.y, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.z, reader);
        StaticNetSerializer_fix.NetDeserialize(ref obj.w, reader);
    }
}
