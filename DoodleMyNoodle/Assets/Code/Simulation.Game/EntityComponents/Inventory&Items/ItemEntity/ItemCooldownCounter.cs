using Unity.Entities;

[System.Serializable]
public struct ItemCooldownTimeCounter : IComponentData
{
    public fix Value;
}

[System.Serializable]
public struct ItemCooldownTurnCounter : IComponentData
{
    public fix Value;
}
