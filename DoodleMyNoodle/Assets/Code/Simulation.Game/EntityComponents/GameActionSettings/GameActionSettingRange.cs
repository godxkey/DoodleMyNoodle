using Unity.Entities;
using UnityEngine;

public struct GameActionSettingRange : IComponentData
{
    public fix Value;

    public static implicit operator fix(GameActionSettingRange val) => val.Value;
    public static implicit operator GameActionSettingRange(fix val) => new GameActionSettingRange() { Value = val };
}
