using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Interacted : IComponentData
{
    public bool Value;

    public static implicit operator bool(Interacted val) => val.Value;
    public static implicit operator Interacted(bool val) => new Interacted() { Value = val };
}