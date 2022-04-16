using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;

public interface IPresentationPostSimTick
{
    void PresentationPostSimulationTick();
}

public interface IPresentationUpdate
{
    void PresentationUpdate();
}

public abstract class GamePresentationSystem<T> : GameSystem<T>, IPresentationPostSimTick, IPresentationUpdate
    where T : GamePresentationSystem<T>
{
    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ExternalSimGameWorldAccessor SimWorld => Cache.SimWorld;
    public Unity.Entities.World PresWorld => PresentationHelpers.PresentationWorld;
    public PresentationEventsWithReadAccess PresentationEvents => SimWorld.PresentationEvents;

    public virtual void PresentationPostSimulationTick() { }
    public virtual void PresentationUpdate() { }
}


public abstract class GamePresentationBehaviour : GameMonoBehaviour, IPresentationPostSimTick, IPresentationUpdate
{
    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ExternalSimGameWorldAccessor SimWorld => Cache.SimWorld;
    public Unity.Entities.World PresWorld => PresentationHelpers.PresentationWorld;
    public PresentationEventsWithReadAccess PresentationEvents => SimWorld.PresentationEvents;

    public virtual void PresentationPostSimulationTick() { }
    public virtual void PresentationUpdate() { }
}
