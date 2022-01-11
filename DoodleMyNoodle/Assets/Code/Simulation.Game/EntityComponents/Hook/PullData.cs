using Unity.Entities;
/// <summary>
/// An enity with PullData will be pulled towards a destination until it reaches it. 
/// </summary>
public struct PullData : IComponentData
{
    public fix2 Destination;
    public fix2 AngledBonusVelocity;
    public fix Speed;
    public fix StartTime;
}