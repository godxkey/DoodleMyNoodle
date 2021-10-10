using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct DestroyAfterDelay : IComponentData
{
    public TimeValue Delay;
    public TimeValue TrackedTime;
}