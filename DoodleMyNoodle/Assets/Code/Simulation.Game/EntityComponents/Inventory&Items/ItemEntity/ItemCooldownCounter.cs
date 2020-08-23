using Unity.Entities;

[System.Serializable]
public struct ItemCooldownCounter : IComponentData
{
    public fix Value;
}
