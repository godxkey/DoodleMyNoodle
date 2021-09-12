using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

[GenerateAuthoringComponent]
public struct SpikeTrap : IComponentData
{
    public fix2 DamageDirection;
}