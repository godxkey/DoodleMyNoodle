using System;
using Unity.Entities;

/// <summary>
/// This component is generally added onto items entity. It references a <see cref="ItemPassiveEffectId"/> type (see <see cref="ItemPassiveEffectBank"/>)
/// </summary>
public struct ItemPassiveEffectId : IBufferElementData, IEquatable<ItemPassiveEffectId>
{
    public ushort Value;

    public static ItemPassiveEffectId Invalid => default;
    public bool IsValid => !Equals(Invalid);

    public bool Equals(ItemPassiveEffectId other)
    {
        return Value == other.Value;
    }
}