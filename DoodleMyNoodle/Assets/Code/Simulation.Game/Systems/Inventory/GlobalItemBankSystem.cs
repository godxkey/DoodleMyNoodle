using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngineX;

public struct GlobalItemBankSimAssetIdElement : IBufferElementData
{
    public SimAssetId Value;

    public static implicit operator SimAssetId(GlobalItemBankSimAssetIdElement val) => val.Value;
    public static implicit operator GlobalItemBankSimAssetIdElement(SimAssetId val) => new GlobalItemBankSimAssetIdElement() { Value = val };
}

public struct GlobalItemBankInstanceElement : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(GlobalItemBankInstanceElement val) => val.Value;
    public static implicit operator GlobalItemBankInstanceElement(Entity val) => new GlobalItemBankInstanceElement() { Value = val };
}

public struct GlobalItemBankPrefabElement : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(GlobalItemBankPrefabElement val) => val.Value;
    public static implicit operator GlobalItemBankPrefabElement(Entity val) => new GlobalItemBankPrefabElement() { Value = val };
}

public struct GlobalItemBankTag : IComponentData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GlobalItemBankSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GlobalItemBankTag>();
    }

    protected override void OnUpdate()
    {
        var itemBuffer = GetAllItemInstances();
        if (itemBuffer.Length == 0 && GetAllItemPrefabs().Length > 0)
        {
            RecreateBuffers();
        }
    }

    private void RecreateBuffers()
    {
        NativeArray<GlobalItemBankPrefabElement> itemPrefabs = GetAllItemPrefabs().ToNativeArray(Allocator.Temp);
        NativeArray<GlobalItemBankInstanceElement> itemInstances = new NativeArray<GlobalItemBankInstanceElement>(itemPrefabs.Length, Allocator.Temp);
        NativeArray<GlobalItemBankSimAssetIdElement> itemIds = new NativeArray<GlobalItemBankSimAssetIdElement>(itemPrefabs.Length, Allocator.Temp);

        // instantiate items
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (HasComponent<SimAssetId>(itemPrefabs[i]))
            {
                itemInstances[i] = EntityManager.Instantiate(itemPrefabs[i]);
                itemIds[i] = GetComponent<SimAssetId>(itemPrefabs[i]);
            }
        }

        // fill buffers
        var instancebuffer = GetAllItemInstances();
        instancebuffer.Clear();
        instancebuffer.AddRange(itemInstances);

        var idBuffer = GetAllItemSimAssetIds();
        idBuffer.Clear();
        idBuffer.AddRange(itemIds);
    }

    public bool IsReady() => HasSingleton<GlobalItemBankTag>();

    public DynamicBuffer<GlobalItemBankPrefabElement> GetAllItemPrefabs()
    {
        if (!IsReady())
        {
            Log.Error("Trying to get item bank before it is spawned");
            return default;
        }

        return GetBuffer<GlobalItemBankPrefabElement>(GetSingletonEntity<GlobalItemBankTag>());
    }

    public DynamicBuffer<GlobalItemBankInstanceElement> GetAllItemInstances()
    {
        if (!IsReady())
        {
            Log.Error("Trying to get item bank before it is spawned");
            return default;
        }

        return GetBuffer<GlobalItemBankInstanceElement>(GetSingletonEntity<GlobalItemBankTag>());
    }

    public DynamicBuffer<GlobalItemBankSimAssetIdElement> GetAllItemSimAssetIds()
    {
        if (!IsReady())
        {
            Log.Error("Trying to get item bank before it is spawned");
            return default;
        }

        return GetBuffer<GlobalItemBankSimAssetIdElement>(GetSingletonEntity<GlobalItemBankTag>());
    }

    public Entity GetItemInstance(SimAssetId simAssetId)
    {
        int index = -1;
        var ids = GetAllItemSimAssetIds();

        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == simAssetId)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Log.Error($"Item instance (id:{simAssetId}) was not found. Is it in the GlobalItemBank like it should?");
            return Entity.Null;
        }

        return GetAllItemInstances()[index];
    }

    public Entity GetItemInstance(Entity itemPrefab)
    {
        if (!HasComponent<Prefab>(itemPrefab))
        {
            Log.Error($"Item {itemPrefab} is not a prefab, or no longer exists");
            return Entity.Null;
        }

        if (!HasComponent<SimAssetId>(itemPrefab))
        {
            Log.Error($"Item {itemPrefab} does not have a simAssetId.");
            return Entity.Null;
        }

         return GetItemInstance(GetComponent<SimAssetId>(itemPrefab));
    }
}
