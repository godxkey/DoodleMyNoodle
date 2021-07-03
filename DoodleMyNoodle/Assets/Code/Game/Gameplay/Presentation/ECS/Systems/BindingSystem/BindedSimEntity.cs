using System.Collections;
using Unity.Entities;

public struct BindedSimEntity : IComponentData
{
    public int Index;
    public int Version;

    public static implicit operator Entity(BindedSimEntity val) => new Entity() { Index = val.Index, Version = val.Version };
    public static implicit operator BindedSimEntity(Entity val) => new BindedSimEntity() { Index = val.Index, Version = val.Version };
}