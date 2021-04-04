using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class ItemInfoBank : GameSystem<ItemInfoBank>
{
    [SerializeField] private ItemAuth _fallbackGameActionAuth;

    public ItemAuth GetGameActionAuthFromID(SimAssetId itemID)
    {
        GameObject itemPrefab = PresentationHelpers.FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            ItemAuth gameActionAuth = itemPrefab.GetComponent<ItemAuth>();
            if (gameActionAuth != null)
            {
                return gameActionAuth;
            }
        }

        return _fallbackGameActionAuth;
    }

    public GameObject GetItemPrefabID(SimAssetId itemID)
    {
        return PresentationHelpers.FindSimAssetPrefab(itemID);
    }
}