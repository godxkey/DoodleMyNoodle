using Unity.Entities;

[GenerateAuthoringComponent]
public struct ExplodeOnDeath : IComponentData
{
    public fix Radius;
    public int Damage;
    public bool DestroyTiles;
}