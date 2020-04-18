using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InventoryDisplaySystem : GameMonoBehaviour
{
    private Entity _localPawn = Entity.Null;

    public override void OnGameStart()
    {
        base.OnGameStart();

        _localPawn = PlayerIdHelpers.GetLocalSimPawnEntity(GameMonoBehaviourHelpers.SimulationWorld);
    }

    public override void OnGameUpdate()
    {
        if(GameMonoBehaviourHelpers.SimulationWorld.TryGetBuffer(_localPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(item.ItemEntity,out SimAssetId itemIDComponent))
                {
                    ItemInfo itemInfo = ItemInfoBank.GetItemInfoFromID(itemIDComponent.Value);
                    // We have the item info !
                }
            }
        }
    }
}
