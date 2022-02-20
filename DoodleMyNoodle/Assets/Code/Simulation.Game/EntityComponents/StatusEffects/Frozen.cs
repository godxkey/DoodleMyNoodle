using Unity.Entities;

[System.Serializable]
public struct Frozen : IComponentData
{
    public TimeValue AppliedTime;
    public TimeValue Duration;
}
