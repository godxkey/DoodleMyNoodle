using Unity.Entities;
using UnityEngine;

public struct ItemTimeCooldownData : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }
}