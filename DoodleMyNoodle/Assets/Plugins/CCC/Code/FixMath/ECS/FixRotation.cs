using System;
using Unity.Entities;

[Serializable]
public struct FixRotation : IComponentData
{
    public FixQuaternion Value;
}