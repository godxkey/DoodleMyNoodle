using System;
using Unity.Entities;

public struct TileActorReference : IBufferElementData, IEquatable<TileActorReference>
{
    public Entity Value;

    public bool Equals(TileActorReference other) => Value == other.Value;
    public static implicit operator Entity(TileActorReference val) => val.Value;
    public static implicit operator TileActorReference(Entity val) => new TileActorReference() { Value = val };
}
