using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct ActionOnContact : IBufferElementData
{
    public enum Filter : byte
    {
        None = 0,
        Allies = 1 << 0,
        Enemies = 1 << 1,
        Terrain = 1 << 2
    }

    public Filter ActionFilter;
    public Entity ActionEntity;
}