using System;
using System.Collections.Generic;
using UnityEngine;

public static class ItemInfoBank
{
    private static Dictionary<int, ItemInfo> s_idToItemInfo = new Dictionary<int, ItemInfo>();

    private static bool s_initialized = false;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        if (s_initialized)
            return;
        s_initialized = true;

        // TODO - Using Resources isn't great, we should change this for something else
        ItemInfo[] ItemsInfo= Resources.FindObjectsOfTypeAll<ItemInfo>();

        foreach (ItemInfo ItemInfo in ItemsInfo)
        {
            s_idToItemInfo.Add(ItemInfo.ID, ItemInfo);
        }
    }

    public static ItemInfo GetItemInfoFromID(int itemID)
    {
        if (s_idToItemInfo.TryGetValue(itemID, out ItemInfo result))
        {
            return result;
        }

        DebugService.LogError($"Failed to find item info from the ID {itemID}");

        return null;
    }

    public static int GetIDFromItemInfo(ItemInfo itemInfo)
    {
        if (s_idToItemInfo.ContainsValue(itemInfo))
        {
            int result = s_idToItemInfo.FindFirstKeyWithValue(itemInfo);
            return result;
        }

        DebugService.LogError($"Failed to find item ID from item info {itemInfo}");

        return int.MaxValue;
    }
}