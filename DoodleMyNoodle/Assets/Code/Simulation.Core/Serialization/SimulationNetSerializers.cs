using System.Runtime.CompilerServices;
using Unity.Entities;

public static class StaticNetSerializer_Unity_Entities_Entity
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSerializedBitSize(ref Entity _) => sizeof(int) * 8 * 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Serialize(ref Entity value, BitStreamWriter writer)
    {
        writer.WriteInt32(value.Index);
        writer.WriteInt32(value.Version);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deserialize(ref Entity value, BitStreamReader reader)
    {
        value.Index = reader.ReadInt32();
        value.Version = reader.ReadInt32();
    }
}