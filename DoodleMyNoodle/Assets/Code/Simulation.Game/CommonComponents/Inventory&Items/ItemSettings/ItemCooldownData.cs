using Unity.Entities;
using UnityEngine;

public struct ItemTimeCooldownData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}

public struct ItemTurnCooldownData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}