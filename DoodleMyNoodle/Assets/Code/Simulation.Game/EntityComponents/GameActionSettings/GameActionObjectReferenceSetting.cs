using Unity.Entities;

[GenerateAuthoringComponent]
public struct GameActionObjectReferenceSetting : IComponentData
{
    public Entity ObjectPrefab;
}