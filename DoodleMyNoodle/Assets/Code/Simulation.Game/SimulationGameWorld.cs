using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public class SimulationGameWorld : SimulationWorld
{
    public FixTimeData TurnTime;
    public FixTimeData RoundTime;

    protected override InternalSimWorldAccessor CreateInternalSimWorldAccessor() => new InternalSimGameWorldAccessor();

    public SimulationGameWorld(string name) : base(name) { }
}

public class InternalSimGameWorldAccessor : InternalSimWorldAccessor, ISimGameWorldReadWriteAccessor
{
    private PresentationEventsWithReadAccess _cachedPresentationEvents;
    public PresentationEvents PresentationEvents => _cachedPresentationEvents ??= GetSimWorld().GetOrCreateSystem<PresentationEventSystem>().PresentationEventsInstance;

    public string GetNameSafe(Entity entity)
    {
        return GetSimWorld().EntityManager.GetNameSafe(entity);
    }
}

public interface ISimGameWorldReadWriteAccessor : ISimWorldReadWriteAccessor, ISimGameWorldReadAccessor
{
    PresentationEvents PresentationEvents { get; }
}


public interface ISimGameWorldReadAccessor : ISimWorldReadAccessor
{
    string GetNameSafe(Entity entity);
}