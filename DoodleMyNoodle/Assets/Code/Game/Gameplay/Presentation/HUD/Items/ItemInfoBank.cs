using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class ItemInfoBank : GameSystem<ItemInfoBank>
{
    [SerializeField] private ItemAuth _fallbackItemAuth;

    public ItemVisualInfo GetItemInfoFromID(SimAssetId itemID)
    {
        GameObject itemPrefab = PresentationHelpers.FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            ItemAuth itemAuth = itemPrefab.GetComponent<ItemAuth>();
            if (itemAuth != null)
            {
                return itemAuth.ItemVisualInfo;
            }
        }

        return _fallbackItemAuth.ItemVisualInfo;
    }

    public ItemAuth GetItemAuthFromID(SimAssetId itemID)
    {
        GameObject itemPrefab = PresentationHelpers.FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            ItemAuth itemAuth = itemPrefab.GetComponent<ItemAuth>();
            if (itemAuth != null)
            {
                return itemAuth;
            }
        }

        return _fallbackItemAuth;
    }

    public GameObject GetItemPrefabID(SimAssetId itemID)
    {
        return PresentationHelpers.FindSimAssetPrefab(itemID);
    }
}