using System;
using Unity.Entities;

[Serializable]
public struct FixTranslation : IComponentData
{
    public fix3 Value;

    public static implicit operator fix3(FixTranslation val) => val.Value;
    public static implicit operator FixTranslation(fix3 val) => new FixTranslation() { Value = val };
}