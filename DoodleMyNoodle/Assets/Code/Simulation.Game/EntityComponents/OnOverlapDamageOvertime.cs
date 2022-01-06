using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

public struct OnOverlapDamageOvertimeSetting : IComponentData
{
    public TimeValue Delay;
    public int Damage;
}

public struct OnOverlapDamageOvertimeState : IComponentData
{
    public TimeValue TrackedTime;
}

public struct OnOverlapDamageOvertimeDamagedEntities : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(OnOverlapDamageOvertimeDamagedEntities val) => val.Value;
    public static implicit operator OnOverlapDamageOvertimeDamagedEntities(Entity val) => new OnOverlapDamageOvertimeDamagedEntities() { Value = val };
}