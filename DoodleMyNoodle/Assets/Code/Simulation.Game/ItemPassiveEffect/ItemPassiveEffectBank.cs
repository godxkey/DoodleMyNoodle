using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class ItemPassiveEffectBank
{
    private static Dictionary<ushort, ItemPassiveEffect> s_idToItemPassiveEffect = new Dictionary<ushort, ItemPassiveEffect>();
    private static Dictionary<string, ItemPassiveEffect> s_nameToItemPassiveEffect = new Dictionary<string, ItemPassiveEffect>();
    private static Dictionary<ItemPassiveEffect, ushort> s_itemPassiveEffectToItem = new Dictionary<ItemPassiveEffect, ushort>();
    private static Dictionary<Type, ushort> s_typeToId = new Dictionary<Type, ushort>();

    private static bool s_initialized = false;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        if (s_initialized)
            return;
        s_initialized = true;

        IEnumerable<Type> itemPassiveEffectTypes = TypeUtility.GetTypesDerivedFrom(typeof(ItemPassiveEffect));

        ushort id = 1; // 0 is invalid
        foreach (Type itemPassiveEffectType in itemPassiveEffectTypes)
        {
            ItemPassiveEffect instance = (ItemPassiveEffect)Activator.CreateInstance(itemPassiveEffectType);
            s_idToItemPassiveEffect.Add(id, instance);
            s_nameToItemPassiveEffect.Add(itemPassiveEffectType.Name, instance);
            s_itemPassiveEffectToItem.Add(instance, id);
            s_typeToId.Add(itemPassiveEffectType, id);

            id++;
        }
    }

    public static ItemPassiveEffectId GetItemPassiveEffectId(string itemPassiveEffectTypeName)
    {
        return GetItemPassiveEffectId(GetItemPassiveEffect(itemPassiveEffectTypeName));
    }

    public static ItemPassiveEffectId GetItemPassiveEffectId<T>() where T : ItemPassiveEffect
    {
        return GetItemPassiveEffectId(typeof(T));
    }

    public static ItemPassiveEffectId GetItemPassiveEffectId(Type itemPassiveEffectType)
    {
        if (s_typeToId.TryGetValue(itemPassiveEffectType, out ushort result))
        {
            return new ItemPassiveEffectId { Value = result };
        }

        Log.Error($"Failed to find action id from type {itemPassiveEffectType}");

        return ItemPassiveEffectId.Invalid;
    }

    public static ItemPassiveEffectId GetItemPassiveEffectId(ItemPassiveEffect itemPassiveEffect)
    {
        if (s_itemPassiveEffectToItem.TryGetValue(itemPassiveEffect, out ushort result))
        {
            return new ItemPassiveEffectId { Value = result };
        }

        Log.Error($"Failed to find action id from action instance {itemPassiveEffect}");

        return ItemPassiveEffectId.Invalid;
    }

    public static ItemPassiveEffect GetItemPassiveEffect(ItemPassiveEffectId id)
    {
        return GetItemPassiveEffect(id.Value);
    }

    public static ItemPassiveEffect GetItemPassiveEffect<T>() where T : ItemPassiveEffect
    {
        return (T)GetItemPassiveEffect(GetItemPassiveEffectId<T>());
    }

    public static ItemPassiveEffect GetItemPassiveEffect(ushort id)
    {
        if (s_idToItemPassiveEffect.TryGetValue(id, out ItemPassiveEffect result))
        {
            return result;
        }

        Log.Error($"Failed to find action from id {id}");

        return null;
    }

    public static ItemPassiveEffect GetItemPassiveEffect(string typeName)
    {
        if (s_nameToItemPassiveEffect.TryGetValue(typeName, out ItemPassiveEffect result))
        {
            return result;
        }

        Log.Error($"Failed to find action from type name {typeName}");

        return null;
    }
}