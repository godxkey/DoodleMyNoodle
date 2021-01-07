using Unity.Entities;

public struct ItemBankPrefabReference : IBufferElementData
{
    public Entity ItemEntityPrefab;

    public static implicit operator Entity(ItemBankPrefabReference val) => val.ItemEntityPrefab;
    public static implicit operator ItemBankPrefabReference(Entity val) => new ItemBankPrefabReference() { ItemEntityPrefab = val };
}