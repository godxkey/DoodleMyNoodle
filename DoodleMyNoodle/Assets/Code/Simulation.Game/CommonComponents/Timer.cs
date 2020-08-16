using Unity.Entities;
using Unity.Mathematics;

public struct Timer : IComponentData
{
    public bool CanCountdown;
    public fix Duration;
    public fix EndTime;
}