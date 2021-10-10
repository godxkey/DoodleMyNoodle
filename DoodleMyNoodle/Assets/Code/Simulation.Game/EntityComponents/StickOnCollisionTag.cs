using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct StickOnCollisionTag : IComponentData
{
    public bool Sticked;
}