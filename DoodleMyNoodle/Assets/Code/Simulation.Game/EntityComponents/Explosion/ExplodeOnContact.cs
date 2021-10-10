using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExplodeOnContact : IComponentData
{
    public fix Radius;
    public int Damage;
}