using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A GameMonoBehaviour is a monobehaviour class that receives common game events
/// </summary>
public class GameMonoBehaviour : MonoBehaviour, IIndexedInList
{
    static List<GameMonoBehaviour> s_registeredBehaviours = new List<GameMonoBehaviour>();
    public static ReadOnlyList<GameMonoBehaviour> RegisteredBehaviours => s_registeredBehaviours.AsReadOnlyNoAlloc();

    public ExternalSimWorldAccessor SimWorld => GameMonoBehaviourHelpers.SimulationWorld;
    public Unity.Entities.World PresWorld => GameMonoBehaviourHelpers.PresentationWorld;

    int IIndexedInList.Index { get; set; }

    protected virtual void Awake()
    {
        s_registeredBehaviours.AddIndexed(this);

        if (Game.Ready)
            OnGameReady();
        if (Game.Started)
            OnGameStart();
    }

    protected virtual void OnDestroy()
    {
        s_registeredBehaviours.RemoveIndexed(this);

        if (ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            OnSafeDestroy();
        }
    }

    public virtual void OnGameUpdate() { }
    public virtual void OnGameFixedUpdate() { }
    public virtual void OnGameReady() { }
    public virtual void OnGameStart() { }
    public virtual void OnSafeDestroy() { }
}
