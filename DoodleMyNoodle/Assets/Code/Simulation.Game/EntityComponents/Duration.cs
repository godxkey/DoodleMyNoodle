using Unity.Entities;

public struct Duration : IComponentData
{
    public fix Value;
    public bool IsSeconds;
    public bool IsTurns;
    public bool IsRounds;
}
