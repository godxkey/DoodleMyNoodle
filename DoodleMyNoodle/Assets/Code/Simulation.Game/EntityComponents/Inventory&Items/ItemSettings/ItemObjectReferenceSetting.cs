using Unity.Entities;

[GenerateAuthoringComponent]
public struct ItemObjectReferenceSetting : IComponentData
{
    public Entity ObjectPrefab;
}