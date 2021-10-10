using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct ExplodeOnProximity : IComponentData
{
    public fix Distance;
    public fix Radius;
    public int Damage;
    public bool DestroyTiles;
    public bool Activated;
}