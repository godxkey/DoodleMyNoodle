using Unity.Entities;
using Unity.Mathematics;

public struct GameActionAnimationTypeData : IComponentData
{
    public int AnimationType;
    public fix Duration;
}