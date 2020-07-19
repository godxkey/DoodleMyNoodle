using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;

public interface IPostSimulationTick
{
    void OnPostSimulationTick();
}

public abstract class GamePresentationSystem<T> : GameSystem<T>, IPostSimulationTick where T : GamePresentationSystem<T>
{
    public GamePresentationCache SimWorldCache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => SimWorldCache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;

    public override void OnGameLateUpdate()
    {
        base.OnGameLateUpdate();

        if (SimWorldCache.Ready)
        {
            OnGamePresentationUpdate();
        }
    }

    public virtual void OnPostSimulationTick() { }

    protected abstract void OnGamePresentationUpdate();
}


public abstract class GamePresentationBehaviour : GameMonoBehaviour, IPostSimulationTick
{
    public GamePresentationCache SimWorldCache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => SimWorldCache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;

    public override void OnGameLateUpdate()
    {
        base.OnGameLateUpdate();

        if (SimWorldCache.Ready)
        {
            OnGamePresentationUpdate();
        }
    }

    public virtual void OnPostSimulationTick() { }

    protected abstract void OnGamePresentationUpdate();
}
