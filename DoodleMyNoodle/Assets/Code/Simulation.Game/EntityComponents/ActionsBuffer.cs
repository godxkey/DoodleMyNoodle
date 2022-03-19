using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct ActionsBufferElement : IBufferElementData
{
    public Entity ActionEntity;
}