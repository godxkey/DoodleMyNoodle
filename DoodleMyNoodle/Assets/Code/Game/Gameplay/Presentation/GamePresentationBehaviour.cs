using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;

public abstract class GamePresentationSystem<T> : GameSystem<T>  where T : GamePresentationSystem<T>
{
    public GamePresentationCache SimWorldCache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => SimWorldCache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;
}


public class GamePresentationBehaviour : GameMonoBehaviour
{
    public GamePresentationCache SimWorldCache => GamePresentationCache.Instance;
    public ExternalSimWorldAccessor SimWorld => SimWorldCache.SimWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;
}
