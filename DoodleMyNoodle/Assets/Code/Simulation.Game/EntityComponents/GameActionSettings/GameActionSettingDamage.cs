using Unity.Entities;
using UnityEngine;

public struct GameActionSettingDamage : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }

    public static implicit operator int(GameActionSettingDamage val) => val.Value;
    public static implicit operator GameActionSettingDamage(int val) => new GameActionSettingDamage() { Value = val };
}