using System;
using Unity.Entities;

/// <summary>
/// This component is generally added onto items entity. It references a <see cref="GameAction"/> type (see <see cref="GameActionBank"/>)
/// </summary>
public struct GameActionId : IComponentData, IEquatable<GameActionId>
{
    public ushort Value;

    public static GameActionId Invalid => default;
    public bool IsValid => !Equals(Invalid);

    public bool Equals(GameActionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is GameActionId castedObj && Equals(castedObj);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(GameActionId a, GameActionId b) => a.Equals(b);
    public static bool operator !=(GameActionId a, GameActionId b) => !a.Equals(b);
}
