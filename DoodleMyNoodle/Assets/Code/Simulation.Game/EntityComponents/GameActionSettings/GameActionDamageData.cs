using Unity.Entities;
using UnityEngine;

public struct GameActionDamageData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}