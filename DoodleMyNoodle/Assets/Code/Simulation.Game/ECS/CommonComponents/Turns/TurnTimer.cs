using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnTimer : IComponentData, IStatFix
{
    public Fix64 Value;

    Fix64 IStatFix.Value { get => Value; set => Value = value; }
}
