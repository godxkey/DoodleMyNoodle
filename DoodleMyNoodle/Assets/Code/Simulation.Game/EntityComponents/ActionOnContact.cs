using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct ActionOnContactBaseData
{
    public byte Id; // unique id in the buffer
    public ActorFilter ActionFilter;
    public Entity ActionEntity;
    public fix SameTargetCooldown;
}

public struct ActionOnColliderContact : IBufferElementData
{
    public ActionOnContactBaseData Data;
}

public struct ActionOnOverlap : IBufferElementData
{
    public ActionOnContactBaseData Data;
    public uint OverlapFilter;
    public fix OverlapRadius;
    public fix Cooldown;
}