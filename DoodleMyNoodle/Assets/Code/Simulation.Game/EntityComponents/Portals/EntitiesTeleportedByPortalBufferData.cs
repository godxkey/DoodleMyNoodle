using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct EntitiesTeleportedByPortalBufferData : IBufferElementData
{
    public Entity entity;
}