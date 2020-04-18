using Unity.Entities;

[GenerateAuthoringComponent]
public struct ItemID : IComponentData
{
    public ushort Value;
}
