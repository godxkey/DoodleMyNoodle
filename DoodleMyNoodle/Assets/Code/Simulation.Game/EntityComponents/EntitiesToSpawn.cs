using System;
using Unity.Entities;

[Serializable]
public struct EntitiesToSpawn : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(EntitiesToSpawn val) => val.Value;
    public static implicit operator EntitiesToSpawn(Entity val) => new EntitiesToSpawn() { Value = val };
}