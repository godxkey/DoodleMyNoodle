using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct Portal : IComponentData 
{
    public fix2 NextPos;
    public Entity NextPortal;
}