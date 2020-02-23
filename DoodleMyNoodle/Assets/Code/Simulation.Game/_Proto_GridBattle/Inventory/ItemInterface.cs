
public interface IItemOnEquip
{
    void OnEquip(SimInventoryComponent Inventory);
}

public interface IItemOnUnequip
{
    void OnUnequip(SimInventoryComponent Inventory);
}

public interface IItemOnUse
{
    void OnUse(SimPlayerActions PlayerActions);
}

public interface IItemOnConsume
{
    void OnConsume(SimPlayerActions PlayerActions);
}