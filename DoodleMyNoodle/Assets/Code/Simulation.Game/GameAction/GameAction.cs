using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class GameAction
{
    public static LogChannel LogChannel = Log.CreateChannel("Game Actions", activeByDefault: true);

    public abstract class ParameterDescription
    {
    }

    [NetSerializable(baseClass = true)]
    public abstract class ParameterData
    {
        public byte ParamIndex;

        public ParameterData() { }

        protected ParameterData(byte paramIndex)
        {
            ParamIndex = paramIndex;
        }
    }

    public sealed class UseContract
    {
        public ParameterDescription[] ParameterTypes;

        public UseContract(params ParameterDescription[] parameterTypes)
        {
            ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));
        }
    }

    [NetSerializable]
    public sealed class UseParameters
    {
        public ParameterData[] ParameterDatas;

        public UseParameters(params ParameterData[] parameterDatas)
        {
            ParameterDatas = parameterDatas ?? throw new ArgumentNullException(nameof(parameterDatas));
        }

        // using this instead with a private constructor will allow us to later use pooling without changing much code
        public static UseParameters Create(params ParameterData[] parameterDescription)
        {
            return new UseParameters(parameterDescription);
        }

        public bool TryGetParameter<T>(int index, out T parameterData) where T : ParameterData
        {
            for (int i = 0; i < ParameterDatas.Length; i++)
            {
                if (ParameterDatas[i] == null)
                {
                    Debug.LogWarning($"GameAction parameters[{i}] is null");
                    continue;
                }

                if (ParameterDatas[i].ParamIndex == index && ParameterDatas[i] is T p)
                {
                    parameterData = p;
                    return true;
                }
            }

            parameterData = null;
            return false;
        }
    }

    public struct UseContext
    {
        public Entity InstigatorPawnController;
        public Entity InstigatorPawn;
        public Entity Entity;
    }

    public bool TryUse(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (!IsContextValid(accessor, context))
        {
            return false;
        }

        Use(accessor, context, parameters);
        return true;
    }

    public abstract void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters);
    public abstract bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogGameActionInfo(in UseContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - context(item: {context.Entity}, instigator: {context.InstigatorPawn}, instigatorController: {context.InstigatorPawnController})");
    }
}
