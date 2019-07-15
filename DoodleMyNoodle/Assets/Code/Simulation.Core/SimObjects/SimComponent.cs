using System;
using UnityEngine;

[RequireComponent(typeof(SimEntity))]
public abstract class SimComponent : MonoBehaviour
{
    // this is to prevent us from accidently using the unity transform instead of the SimTransform
    [NonSerialized]
    public new bool transform = false;

    /// <summary>
    /// Similar to unity's Awake() but only called when the entity actually becomes part of the simulation
    /// <para/>
    /// NB: If the game is saved and reloaded, this will NOT be called again. Use OnAddedToEntityList for that instead
    /// </summary>
    public virtual void OnSimAwake() { }

    /// <summary>
    /// Similar to unity's OnDestroy(). Called after calling SimWorld.Destroy(entity)
    /// <para/>
    /// NB: If the game is Saved & Exited, this will NOT be called. Use OnRemovingFromEntityList for that instead
    /// </summary>
    public virtual void OnSimDestroy() { }

    /// <summary>
    /// Called when the object is added to the simulation runtime. Contrary to OnSimAwake, it's called every time the game is reloaded
    /// </summary>
    public virtual void OnAddedToEntityList() { }
    /// <summary>
    /// Called when the object is removed from the simulation runtime. Contrary to OnSimDestroy, it's called every time the game exited
    /// </summary>
    public virtual void OnRemovingFromEntityList() { }

    public Transform unityTransform => gameObject.transform;
    public SimTransform simTransform
    {
        get
        {
            if (_cachedSimTransform == null)
            {
                _cachedSimTransform = GetComponent<SimTransform>();
            }

            return _cachedSimTransform;
        }
    }
    public SimEntity simEntity
    {
        get
        {
            if (_cachedSimEntity == null)
            {
                _cachedSimEntity = GetComponent<SimEntity>();
            }

            return _cachedSimEntity;
        }
    }

    [NonSerialized]
    SimTransform _cachedSimTransform;
    [NonSerialized]
    SimEntity _cachedSimEntity;
}
