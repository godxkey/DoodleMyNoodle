using Unity.Entities;
using Unity.Mathematics;

public struct PlayerPawnPrefabReferenceSingletonComponent : IComponentData
{
    public Entity Prefab;
}