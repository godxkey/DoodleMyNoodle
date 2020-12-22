using Unity.Entities;
using UnityEngine;

public struct GameActionHPToHealData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}