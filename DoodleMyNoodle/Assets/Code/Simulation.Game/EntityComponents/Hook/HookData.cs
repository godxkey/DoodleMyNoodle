using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;


/// <summary>
/// An object with HookData will travel forward until it collides with an entity. 
/// After doing so, it will travel back to its start position and drag the other entity along.
/// </summary>
public struct HookData : IComponentData
{
    public enum EState : byte
    {
        Uninitialized,
        TravelingForward,
        TravelingBack,
    }

    public fix2 StartPosition;
    public fix TravelBackSpeed;
    public Entity TouchedEntity;
    public EState State;
}