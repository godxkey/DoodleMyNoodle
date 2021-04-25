using Unity.Entities;
using UnityEngine;

public struct GameActionDamageData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }

    public static implicit operator int(GameActionDamageData val) => val.Value;
    public static implicit operator GameActionDamageData(int val) => new GameActionDamageData() { Value = val };
}