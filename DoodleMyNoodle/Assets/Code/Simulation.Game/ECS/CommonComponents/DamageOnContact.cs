using Unity.Entities;

[GenerateAuthoringComponent]
public struct DamageOnContact : IComponentData
{
    public int Value;
    public bool DestroySelf;
}