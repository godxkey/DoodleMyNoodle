using Unity.Entities;
using Unity.Mathematics;

public struct InteractableInventoryPrefabReference : IComponentData
{
    public Entity Prefab;
}