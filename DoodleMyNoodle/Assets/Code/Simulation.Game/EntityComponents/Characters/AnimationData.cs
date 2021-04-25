using Unity.Entities;
using Unity.Mathematics;

public struct AnimationData : IComponentData
{
    public fix2 Direction;
    public fix TotalDuration;
    public fix LastTransitionTime;
    public Entity GameActionEntity;
}