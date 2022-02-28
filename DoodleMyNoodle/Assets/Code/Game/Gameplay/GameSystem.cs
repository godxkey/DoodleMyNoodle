using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using UnityEngineX.InspectorDisplay;

public enum GameSystemType
{
    Core,
    GameplayPresentation
}

[Flags]
public enum GameSystemOnlineFlags
{
    Local = 1 << 0,
    Client = 1 << 1,
    Server = 1 << 2
}

/// <summary>
/// A Game System is a singleton logic class accessible ingame. The game will only start when all GameSystems are ready
/// </summary>
public class GameSystem : GameMonoBehaviour
{
    [Serializable]
    public class Settings
    {
        public GameSystemType Type = GameSystemType.GameplayPresentation;

        [ShowIf(nameof(IsCoreType))]
        public GameSystemOnlineFlags OnlineFlags = (GameSystemOnlineFlags)~0;

        private bool IsCoreType => Type == GameSystemType.Core;
    }

    [Header("Game System")]
    [AlwaysExpand]
    public Settings SystemSettings;

    public static List<GameSystem> UnreadySystems = new List<GameSystem>();

    public virtual bool SystemReady { get; } = true;

    protected override void Awake()
    {
        base.Awake();

        if (SystemReady == false)
        {
            UnreadySystems.Add(this);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnreadySystems.Remove(this);
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