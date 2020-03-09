
using System;

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
    void OnUse(SimPlayerActions PlayerActions, object[] Informations);
}

public interface IItemOnConsume
{
    void OnConsume(SimPlayerActions PlayerActions);
}

public interface IItemTryGetUsageContext
{
    void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady);
}