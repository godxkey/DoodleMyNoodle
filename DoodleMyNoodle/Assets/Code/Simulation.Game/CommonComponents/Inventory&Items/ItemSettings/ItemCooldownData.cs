using Unity.Entities;

[GenerateAuthoringComponent]
public struct ItemCooldownData : IComponentData
{
    public int Value;
}
