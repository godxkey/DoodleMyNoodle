using Unity.Entities;
using UnityEngine;

public struct ActionSettingAPCost : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}