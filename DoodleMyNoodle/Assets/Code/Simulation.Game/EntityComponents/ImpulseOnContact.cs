using Unity.Entities;

[GenerateAuthoringComponent]
public struct ImpulseOnContact : IComponentData
{
    public fix Strength;
}