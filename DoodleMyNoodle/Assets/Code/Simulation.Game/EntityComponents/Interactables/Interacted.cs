using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Interacted : IComponentData
{
    public bool Value;
    public Entity Instigator;

    public static implicit operator bool(Interacted interacted) => interacted.Value;
}