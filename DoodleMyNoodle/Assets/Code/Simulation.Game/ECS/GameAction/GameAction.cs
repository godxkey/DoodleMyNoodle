using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class GameAction
{
    public abstract class ParameterDescription
    {
    }

    [NetSerializable(baseClass = true)]
    public abstract class ParameterData
    {
        public int ParamIndex;

        public ParameterData() { }

        protected ParameterData(int paramIndex)
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
        public Entity ItemEntity;
    }

    public abstract void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters);
    public abstract bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context);
}
