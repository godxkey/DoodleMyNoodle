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
    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => Cache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;

    public override void OnGameLateUpdate()
    {
        base.OnGameLateUpdate();

        if (Cache.Ready)
        {
            OnGamePresentationUpdate();
        }
    }

    public virtual void OnPostSimulationTick() { }
    protected virtual void OnGamePresentationUpdate() { }
}


public abstract class GamePresentationBehaviour : GameMonoBehaviour, IPostSimulationTick
{
    public GamePresentationCache Cache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => Cache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;

    public override void OnGameLateUpdate()
    {
        base.OnGameLateUpdate();

        if (Cache.Ready)
        {
            OnGamePresentationUpdate();
        }
    }

    public virtual void OnPostSimulationTick() { }

    protected abstract void OnGamePresentationUpdate();
}
