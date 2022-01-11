using Unity.Entities;
using static fixMath;

/// <summary>
/// This component is added onto AI controllers (equivalent of Player entity)
/// </summary>
public struct AITag : IComponentData
{
}

/// <summary>
/// The state of the AI
/// </summary>
public struct AIState : IComponentData
{
    public AIStateEnum Value;

    public static implicit operator AIStateEnum(AIState val) => val.Value;
    public static implicit operator AIState(AIStateEnum val) => new AIState() { Value = val };
}
public enum AIStateEnum { Patrol, Combat }

/// <summary>
/// Indicates whether the AI is supposed to wait a certain time before playing
/// </summary>
public struct AIActionCooldown : IComponentData
{
    public fix NoActionUntilTime;
}

/// <summary>
/// Indicates if an AI should play this frame
/// </summary>
public struct AIThinksThisFrameToken : IComponentData
{
    public bool Value;

    public static implicit operator bool(AIThinksThisFrameToken val) => val.Value;
    public static implicit operator AIThinksThisFrameToken(bool val) => new AIThinksThisFrameToken() { Value = val };
}

/// <summary>
/// Stores data about the last move input of the AI
/// </summary>
public struct AIMoveInputLastFrame : IComponentData
{
    public bool WasAttemptingToMove;
}

/// <summary>
/// The destination of an AI
/// </summary>
public struct AIDestination : IComponentData
{
    public fix2 Position;
    public bool HasDestination;
}

/// <summary>
/// Cached data to avoid constant repaths
/// </summary>
public struct AIDestinationRepathData : IComponentData
{
    public fix2 PathCreatedPosition;
    public fix PathCreatedTime;
}

/// <summary>
/// A buffer of position for AI pathing
/// </summary>
public struct AIPathSegment : IBufferElementData
{
    public Pathfinding.Segment Value;

    public static implicit operator Pathfinding.Segment(AIPathSegment val) => val.Value;
    public static implicit operator AIPathSegment(Pathfinding.Segment val) => new AIPathSegment() { Value = val };
}

/// <summary>
/// Data for patroling AI
/// </summary>
public struct AIPatrolData : IComponentData
{
    public TimeValue LastPatrolTurn;
}

public struct AIFuzzyThrowSettings : IComponentData
{
    public FixFuzzyValue ThrowSpeed;
    public FixFuzzyValue ThrowAngle;
}