using System;
using Unity.Entities;

[Serializable]
[NetSerializable]
[GenerateAuthoringComponent]
public struct PersistentId : IComponentData, IEquatable<PersistentId>
{
    public uint Value;


    public static PersistentId Invalid => default;

    public override bool Equals(object obj) => Equals((PersistentId)obj);
    public override int GetHashCode() => Value.GetHashCode();
    public bool Equals(PersistentId other) => other.Value == Value;
    public static bool operator ==(PersistentId a, PersistentId b) => a.Equals(b);
    public static bool operator !=(PersistentId a, PersistentId b) => !(a.Value == b.Value);
    public override string ToString()
    {
        if (this == Invalid)
            return "PersistentId(Invalid)";
        return $"PersistentId({Value})";
    }
}

[Serializable]
public struct NextPersistentId : IComponentData
{
    public PersistentId NextId;
}


public static class PersistentIdExtensions
{
    public static PersistentId MakeUniquePersistentId(this ComponentSystem componentSystem)
    {
        NextPersistentId nextPersistentId = componentSystem.GetOrCreateSingleton<NextPersistentId>();

        nextPersistentId.NextId.Value++;

        if (nextPersistentId.NextId == PersistentId.Invalid)
            nextPersistentId.NextId.Value++;

        componentSystem.SetOrCreateSingleton(nextPersistentId);

        return nextPersistentId.NextId;
    }

    public static PersistentId MakeUniquePersistentId(this CCCSystemBase system)
    {
        NextPersistentId nextPersistentId = system.GetOrCreateSingleton<NextPersistentId>();

        nextPersistentId.NextId.Value++;

        if (nextPersistentId.NextId == PersistentId.Invalid)
            nextPersistentId.NextId.Value++;

        system.SetOrCreateSingleton(nextPersistentId);

        return nextPersistentId.NextId;
    }

    public static PersistentId MakeUniquePersistentId(this ISimWorldReadWriteAccessor accessor)
    {
        // we assume next persisten id exist
        NextPersistentId nextPersistentId = accessor.GetOrCreateSingleton<NextPersistentId>();
        nextPersistentId.NextId.Value++;

        if (nextPersistentId.NextId == PersistentId.Invalid)
            nextPersistentId.NextId.Value++;

        accessor.SetOrCreateSingleton(nextPersistentId);

        return nextPersistentId.NextId;
    }
}