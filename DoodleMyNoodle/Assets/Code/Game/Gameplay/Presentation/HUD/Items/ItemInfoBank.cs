using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class ItemInfoBank : GameSystem<ItemInfoBank>
{
    [SerializeField] private GameActionAuth _fallbackGameActionAuth;

    public GameActionAuth GetGameActionAuthFromID(SimAssetId itemID)
    {
        GameObject itemPrefab = PresentationHelpers.FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            GameActionAuth gameActionAuth = itemPrefab.GetComponent<GameActionAuth>();
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