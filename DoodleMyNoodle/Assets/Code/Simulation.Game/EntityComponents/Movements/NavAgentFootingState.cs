using Unity.Entities;
using Unity.Mathematics;

public enum NavAgentFooting
{
    /// <summary>
    /// Agent is stable on ground
    /// </summary>
    Ground,

    /// <summary>
    /// Agent is stable in ladder
    /// </summary>
    Ladder,

    /// <summary>
    /// Agent has no footing (falling in slope, moving in mid-air)
    /// </summary>
    None,
}

public struct NavAgentFootingState : IComponentData
{
    public NavAgentFooting Value;

    public static implicit operator NavAgentFooting(NavAgentFootingState val) => val.Value;
    public static implicit operator NavAgentFootingState(NavAgentFooting val) => new NavAgentFootingState() { Value = val };
}