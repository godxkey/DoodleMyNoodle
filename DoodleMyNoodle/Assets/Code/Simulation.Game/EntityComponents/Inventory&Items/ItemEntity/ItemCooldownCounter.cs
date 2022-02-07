using Unity.Entities;

[System.Serializable]
public struct ItemCooldownTimeCounter : IComponentData
{
    public fix Value;

    public static implicit operator fix(ItemCooldownTimeCounter val) => val.Value;
    public static implicit operator ItemCooldownTimeCounter(fix val) => new ItemCooldownTimeCounter() { Value = val };
}