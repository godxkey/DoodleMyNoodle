// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_PersistentId
{
    public static int GetNetBitSize(ref PersistentId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref PersistentId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref PersistentId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.Value, reader);
    }
}
public static class StaticNetSerializer_SimCheatInput
{
    public static int GetNetBitSize_Class(SimCheatInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCheatInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCheatInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCheatInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimCheatInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimCheatInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimCheatInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimCommand
{
    public static int GetNetBitSize_Class(SimCommand obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommand obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommand obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommand obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimCommand NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimCommand)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimCommand obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInput
{
    public static int GetNetBitSize_Class(SimInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInput obj)
    {
        int result = 0;
        return result;
    }

    public static void NetSerialize_Class(SimInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInput obj, BitStreamWriter writer)
    {
    }

    public static SimInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimInput obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_SimMasterInput
{
    public static int GetNetBitSize_Class(SimMasterInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimMasterInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimMasterInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimMasterInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimMasterInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimMasterInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimMasterInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
