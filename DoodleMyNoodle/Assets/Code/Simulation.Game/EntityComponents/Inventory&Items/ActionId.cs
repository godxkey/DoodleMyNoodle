using System;
using Unity.Entities;

/// <summary>
/// This component is generally added onto items entity. It references a <see cref="Action"/> type (see <see cref="ActionBank"/>)
/// </summary>
public struct ActionId : IComponentData, IEquatable<ActionId>
{
    public ushort Value;

    public static ActionId Invalid => default;
    public bool IsValid => !Equals(Invalid);

    public bool Equals(ActionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is ActionId castedObj && Equals(castedObj);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(ActionId a, ActionId b) => a.Equals(b);
    public static bool operator !=(ActionId a, ActionId b) => !a.Equals(b);
}
