using System.Collections;
using System.Collections.Generic;
using UnityX;

/// <summary>
/// A Game System is a singleton logic class accessible ingame. The game will only start when all GameSystems are ready
/// </summary>
public abstract class GameSystem : GameMonoBehaviour
{
    public static List<GameSystem> unreadySystems = new List<GameSystem>();

    public abstract bool SystemReady { get; }

    protected override void Awake()
    {
        base.Awake();

        if (SystemReady == false)
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
    public static T Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
            Log.Error("We have 2 instances of " + typeof(T).Name);

        Instance = (T)this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Instance = null;
    }
}