using Unity.Entities;

[System.Serializable]
public struct ItemCooldownTimeCounter : IComponentData
{
    public fix Value;

    public static implicit operator fix(ItemCooldownTimeCounter val) => val.Value;
    public static implicit operator ItemCooldownTimeCounter(fix val) => new ItemCooldownTimeCounter() { Value = val };
}

[System.Serializable]
public struct ItemCooldownTurnCounter : IComponentData
{
    public int Value;

    public static implicit operator int(ItemCooldownTurnCounter val) => val.Value;
    public static implicit operator ItemCooldownTurnCounter(int val) => new ItemCooldownTurnCounter() { Value = val };
}
