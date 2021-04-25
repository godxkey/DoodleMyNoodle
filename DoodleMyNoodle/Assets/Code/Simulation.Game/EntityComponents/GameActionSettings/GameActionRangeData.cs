using Unity.Entities;
using UnityEngine;

public struct GameActionRangeData : IComponentData
{
    public fix Value;

    public static implicit operator fix(GameActionRangeData val) => val.Value;
    public static implicit operator GameActionRangeData(fix val) => new GameActionRangeData() { Value = val };
}
