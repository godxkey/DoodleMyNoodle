using Unity.Entities;
using Unity.Mathematics;

public struct NoInteractTimer : IComponentData
{
    public bool CanCountdown;
    public fix Duration;
    public fix EndTime;
}