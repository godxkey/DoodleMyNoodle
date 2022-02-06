using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngineX;

public static class ActionBank
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

    private static Dictionary<ushort, Action> s_idToGameAction = new Dictionary<ushort, Action>();
    private static Dictionary<string, Action> s_nameToGameAction = new Dictionary<string, Action>();

    private static bool s_initialized = false;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        if (s_initialized)
            return;
        s_initialized = true;

        IEnumerable<Type> gameActionTypes = TypeUtility.GetTypesDerivedFrom(typeof(Action));

        ushort id = 1; // 0 is invalid
        foreach (Type gameActionType in gameActionTypes)
        {
            if (gameActionType.IsAbstract)
                continue;

            Action instance = (Action)Activator.CreateInstance(gameActionType);

            TypeId.Get(gameActionType) = id;

            s_idToGameAction.Add(id, instance);
            s_nameToGameAction.Add(gameActionType.Name, instance);

            id++;
        }
    }

    public static ActionId GetActionId(string gameActionTypeName)
    {
        return GetActionId(GetAction(gameActionTypeName));
    }

    public static ActionId GetActionId<T>() where T : Action
    {
        return new ActionId { Value = TypeId<T>.Ref.Data };
    }

    public static ActionId GetActionId(Type gameActionType)
    {
        return new ActionId { Value = TypeId.Get(gameActionType) };
    }

    public static ActionId GetActionId(Action gameAction)
    {
        return GetActionId(gameAction.GetType());
    }

    public static Action GetAction(ActionId id)
    {
        return GetAction(id.Value);
    }

    public static Action GetAction<T>() where T : Action
    {
        return (T)GetAction(GetActionId<T>());
    }

    public static Action GetAction(ushort id)
    {
        if (s_idToGameAction.TryGetValue(id, out Action result))
        {
            return result;
        }

        Log.Error($"Failed to find action from id {id}");

        return null;
    }

    public static Action GetAction(string typeName)
    {
        if (s_nameToGameAction.TryGetValue(typeName, out Action result))
        {
            return result;
        }

        Log.Error($"Failed to find action from type name {typeName}");

        return null;
    }
}