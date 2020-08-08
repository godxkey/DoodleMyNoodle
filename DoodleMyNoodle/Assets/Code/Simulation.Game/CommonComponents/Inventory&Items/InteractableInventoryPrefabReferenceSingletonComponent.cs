using Unity.Entities;
using Unity.Mathematics;

public struct InteractableInventoryPrefabReferenceSingletonComponent : IComponentData
{
    public Entity Prefab;
}