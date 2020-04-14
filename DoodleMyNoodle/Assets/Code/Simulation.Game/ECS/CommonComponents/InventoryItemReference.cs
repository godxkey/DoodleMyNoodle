using Unity.Entities;

public struct InventoryItemReference : IBufferElementData
{
    public Entity ItemEntity;
}

public struct StartingInventoryItem : IBufferElementData
{
    public Entity ItemEntityPrefab;
}