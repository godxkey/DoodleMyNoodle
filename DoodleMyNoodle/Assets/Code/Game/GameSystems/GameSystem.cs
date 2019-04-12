using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Game System is a singleton logic class accessible ingame. The game will only start when all GameSystems are ready
/// </summary>
public abstract class GameSystem : GameMonoBehaviour
{
    public static List<GameSystem> unreadySystems = new List<GameSystem>();

    public abstract bool isSystemReady { get; }

    protected override void Awake()
    {
        base.Awake();

        if (isSystemReady == false)
        {
            unreadySystems.Add(this);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        unreadySystems.Remove(this);
    }

}


/// <summary>
/// A Game System is a singleton logic class accessible ingame. The game will only start when all GameSystems are ready
/// </summary>
public abstract class GameSystem<T> : GameSystem where T : GameSystem<T>
{
    public static T instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (instance != null)
            DebugService.LogError("We have 2 instances of " + nameof(T));

        instance = (T)this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        instance = null;
    }
}