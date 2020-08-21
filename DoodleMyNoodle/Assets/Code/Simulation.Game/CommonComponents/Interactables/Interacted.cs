using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Interacted : IComponentData
{
    public bool Value;

    public static implicit operator bool(Interacted interacted) => interacted.Value;
    public static implicit operator Interacted(bool interacted) => new Interacted() { Value = interacted };
}