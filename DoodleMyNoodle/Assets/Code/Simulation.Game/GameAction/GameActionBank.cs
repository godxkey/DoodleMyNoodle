using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngineX;

public static class GameActionBank
{
    private sealed class GameActionBankKeyContext
    {
        private GameActionBankKeyContext() { }
    }
    private sealed class TypeId
    {
        public static ref ushort Get(Type componentType)
        {
            return ref SharedStatic<ushort>.GetOrCreate(typeof(GameActionBankKeyContext), componentType).Data;
        }
    }
    private sealed class TypeId<TGameAction>
    {
        public static readonly SharedStatic<ushort> Ref = SharedStatic<ushort>.GetOrCreate<GameActionBankKeyContext, TGameAction>();
    }

    private static Dictionary<ushort, GameAction> s_idToGameAction = new Dictionary<ushort, GameAction>();
    private static Dictionary<string, GameAction> s_nameToGameAction = new Dictionary<string, GameAction>();

    private static bool s_initialized = false;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        if (s_initialized)
            return;
        s_initialized = true;

        IEnumerable<Type> gameActionTypes = TypeUtility.GetTypesDerivedFrom(typeof(GameAction));

        ushort id = 1; // 0 is invalid
        foreach (Type gameActionType in gameActionTypes)
        {
            if (gameActionType.IsAbstract)
                continue;

            GameAction instance = (GameAction)Activator.CreateInstance(gameActionType);

            TypeId.Get(gameActionType) = id;

            s_idToGameAction.Add(id, instance);
            s_nameToGameAction.Add(gameActionType.Name, instance);

            id++;
        }
    }

    public static GameActionId GetActionId(string gameActionTypeName)
    {
        return GetActionId(GetAction(gameActionTypeName));
    }

    public static GameActionId GetActionId<T>() where T : GameAction
    {
        return new GameActionId { Value = TypeId<T>.Ref.Data };
    }

    public static GameActionId GetActionId(Type gameActionType)
    {
        return new GameActionId { Value = TypeId.Get(gameActionType) };
    }

    public static GameActionId GetActionId(GameAction gameAction)
    {
        return GetActionId(gameAction.GetType());
    }

    public static GameAction GetAction(GameActionId id)
    {
        return GetAction(id.Value);
    }

    public static GameAction GetAction<T>() where T : GameAction
    {
        return (T)GetAction(GetActionId<T>());
    }

    public static GameAction GetAction(ushort id)
    {
        if (s_idToGameAction.TryGetValue(id, out GameAction result))
        {
            return result;
        }

        Log.Error($"Failed to find action from id {id}");

        return null;
    }

    public static GameAction GetAction(string typeName)
    {
        if (s_nameToGameAction.TryGetValue(typeName, out GameAction result))
        {
            return result;
        }

        Log.Error($"Failed to find action from type name {typeName}");

        return null;
    }
}