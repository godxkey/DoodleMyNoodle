using Unity.Mathematics;
using CCC.Fix2D;
using Unity.Entities;

public struct EffectGroupBufferSingleton : ISingletonBufferElementData
{
    public uint ID;
    public Entity Entity;
    public fix TimeStamp;
    public fix Delay;
}