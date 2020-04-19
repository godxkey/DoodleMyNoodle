using System;
using Unity.Entities;

/// <summary>
/// This component is generally added onto items entity. It references a <see cref="GameAction"/> type (see <see cref="GameActionBank"/>)
/// </summary>
public struct GameActionId : IComponentData, IEquatable<GameActionId>
{
    public ushort Value;

    public static GameActionId Invalid => default;
    public bool IsValid => Equals(Invalid);

    public bool Equals(GameActionId other)
    {
        return Value == other.Value;
    }
}
