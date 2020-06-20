using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using UnityX;

public static class GameActionBank
{
    private static Dictionary<ushort, GameAction> s_idToGameAction = new Dictionary<ushort, GameAction>();
    private static Dictionary<string, GameAction> s_nameToGameAction = new Dictionary<string, GameAction>();
    private static Dictionary<GameAction, ushort> s_gameActionToId = new Dictionary<GameAction, ushort>();
    private static Dictionary<Type, ushort> s_typeToId = new Dictionary<Type, ushort>();

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
            GameAction instance = (GameAction)Activator.CreateInstance(gameActionType);
            s_idToGameAction.Add(id, instance);
            s_nameToGameAction.Add(gameActionType.Name, instance);
            s_gameActionToId.Add(instance, id);
            s_typeToId.Add(gameActionType, id);

            id++;
        }
    }

    public static GameActionId GetActionId(string gameActionTypeName)
    {
        return GetActionId(GetAction(gameActionTypeName));
    }

    public static GameActionId GetActionId<T>() where T : GameAction
    {
        return GetActionId(typeof(T));
    }

    public static GameActionId GetActionId(Type gameActionType)
    {
        if (s_typeToId.TryGetValue(gameActionType, out ushort result))
        {
            return new GameActionId { Value = result };
        }

        Log.Error($"Failed to find action id from type {gameActionType}");

        return GameActionId.Invalid;
    }

    public static GameActionId GetActionId(GameAction gameAction)
    {
        if (s_gameActionToId.TryGetValue(gameAction, out ushort result))
        {
            return new GameActionId { Value = result };
        }

        Log.Error($"Failed to find action id from action instance {gameAction}");

        return GameActionId.Invalid;
    }

    public static GameAction GetAction(GameActionId id)
    {
        return GetAction(id.Value);
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