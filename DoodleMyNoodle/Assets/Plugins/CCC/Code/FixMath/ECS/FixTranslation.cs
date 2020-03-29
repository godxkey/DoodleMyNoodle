using System;
using Unity.Entities;

[Serializable]
public struct FixTranslation : IComponentData
{
    public fix3 Value;
}