using System;
using UnityEngine;

public abstract class SimObject : MonoBehaviour
{
#if UNITY_EDITOR
    // this is to prevent us from accidently using the unity transform instead of the SimTransform
    [NonSerialized]
    public new bool transform = false;
#endif

    /// <summary>
    /// Similar to unity's Awake() but only called when the entity actually becomes part of the simulation
    /// <para/>
    /// NB: If the game is saved and reloaded, this will NOT be called again. Use OnAddedToRuntime for that instead
    /// </summary>
    public virtual void OnSimAwake() { }

    /// <summary>
    /// Equivalent to unity's Start()
    /// <para/>
    /// NB: If the game is saved and reloaded, this will NOT be called again. Use OnAddedToRuntime for that instead
    /// </summary>
    public virtual void OnSimStart() { }

    /// <summary>
    /// Similar to unity's OnDestroy(). Called after calling SimWorld.Destroy(entity)
    /// <para/>
    /// NB: If the game is Saved & Exited, this will NOT be called. Use OnRemovingFromRuntime for that instead
    /// </summary>
    public virtual void OnSimDestroy() { }

    /// <summary>
    /// Called when the object is added to the simulation runtime. Contrary to OnSimAwake, it's called every time the game is reloaded.
    /// <para/>
    /// DO NOT USE THIS FOR GAME LOGIC. Use OnSimAwake instead. This should be used exclusively for things like caching non-serialized variables
    /// </summary>
    public virtual void OnAddedToRuntime() { }
    /// <summary>
    /// Called when the object is removed from the simulation runtime. Contrary to OnSimDestroy, it's called every time the game exited
    /// <para/>
    /// DO NOT USE THIS FOR GAME LOGIC. Use OnSimDestroy instead
    /// </summary>
    public virtual void OnRemovingFromRuntime() { }

    public Transform UnityTransform => gameObject.transform;
    public SimTransformComponent SimTransform
    {
        get
        {
            if (_cachedSimTransform == null)
            {
                _cachedSimTransform = GetComponent<SimTransformComponent>();
            }

            return _cachedSimTransform;
        }
    }

    [NonSerialized]
    SimTransformComponent _cachedSimTransform;
}
