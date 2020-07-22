using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Interacted : IComponentData
{
    public bool Value;   
}