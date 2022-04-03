using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngineX;

public struct GameFunctionId : IComponentData, IEquatable<GameFunctionId>
{
    public ushort Value;

    public static GameFunctionId Invalid => default;
    public bool IsValid => !Equals(Invalid);

    public bool Equals(GameFunctionId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is GameFunctionId castedObj && Equals(castedObj);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(GameFunctionId a, GameFunctionId b) => a.Equals(b);
    public static bool operator !=(GameFunctionId a, GameFunctionId b) => !a.Equals(b);
}
public delegate void GameFunction<T>(ref T arg);

[AttributeUsage(AttributeTargets.Field)]
public class RegisterGameFunctionAttribute : Attribute
{

}

public class GameFunctions
{

    public static GameFunctionId GetId<T>(T function) where T : Delegate
    {
        if (s_function2Id.TryGetValue(function, out GameFunctionId id))
            return id;

        Log.Warning($"Could not find id for GameFunction {function}. Is your function a static field with [RegisterGameFunction]?");
        return GameFunctionId.Invalid;
    }

    public static void Execute<T>(GameFunctionId functionId, ref T argument)
    {
        if (s_id2Function.TryGetValue(functionId, out Delegate uncastedFunction))
        {
            (uncastedFunction as GameFunction<T>).Invoke(ref argument);
        }
        else
        {
            Log.Warning($"Could not find GameFunction with id {functionId.Value}");
        }
    }


    private static Dictionary<GameFunctionId, Delegate> s_id2Function = new Dictionary<GameFunctionId, Delegate>();
    private static Dictionary<Delegate, GameFunctionId> s_function2Id = new Dictionary<Delegate, GameFunctionId>();
    private static bool s_initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Initialize()
    {
        if (s_initialized)
            return;

        s_initialized = true;
        s_id2Function.Clear();
        s_function2Id.Clear();

        var gameActionGenDefType = typeof(GameFunction<int>).GetGenericTypeDefinition();

        ushort idIterator = 1; // 0 is invalid
        foreach (var item in TypeUtility.GetStaticFieldsWithAttribute(typeof(RegisterGameFunctionAttribute)))
        {
            if (!item.IsStatic)
            {
                LogFailureReason("Field is not static.");
                continue;
            }

            if (!item.FieldType.IsGenericType)
            {
                LogFailureReason("Field is not a GameFunction<T>.");
                continue;
            }

            if (item.FieldType.GetGenericTypeDefinition() != gameActionGenDefType)
            {
                LogFailureReason("Field is not a GameFunction<T>.");
                continue;
            }

            if ((item.Attributes & System.Reflection.FieldAttributes.InitOnly) == 0)
            {
                LogFailureReason("Field needs to be readonly.");
                continue;
            }

            var id = new GameFunctionId() { Value = idIterator };
            var function = item.GetValue(null) as Delegate;
            s_id2Function.Add(id, function);
            s_function2Id.Add(function, id);
            idIterator++;

            void LogFailureReason(string reason)
            {
                Log.Warning($"Failed to register game function {item.Name}: {reason}");
            }
        }

    }
}