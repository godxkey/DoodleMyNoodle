using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

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
}

public interface ISimGameWorldReadWriteAccessor : ISimWorldReadWriteAccessor
{
    PresentationEvents PresentationEvents { get; }
}
