using Unity.Entities;

/// <summary>
/// This component is generally added onto items entity. It references a <see cref="GameAction"/> type (see <see cref="GameActionBank"/>)
/// </summary>
public struct GameActionId : IComponentData
{
    public ushort Value;
}
