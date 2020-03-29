using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TurnTimer : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }
}
