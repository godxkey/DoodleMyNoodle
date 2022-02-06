using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

public class SimulationGameWorld : SimulationWorld
{
    public FixTimeData TurnTime;
    public FixTimeData RoundTime;

    private PresentationEventsWithReadAccess _cachedPresentationEvents;
    public PresentationEventsWithReadAccess PresentationEvents => _cachedPresentationEvents ??= GetOrCreateSystem<PresentationEventSystem>().PresentationEventsInstance;

    public SimulationGameWorld(string name) : base(name) { }
}