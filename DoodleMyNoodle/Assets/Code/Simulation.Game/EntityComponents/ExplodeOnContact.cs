using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExplodeOnContact : IComponentData
{
    public int Range;
    public int Damage;
}