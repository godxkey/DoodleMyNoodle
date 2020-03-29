using System;
using Unity.Entities;

[Serializable]
public struct FixTranslation : IComponentData
{
    public FixVector3 Value;
}