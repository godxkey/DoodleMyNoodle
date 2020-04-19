using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public abstract class GameAction
{
    public abstract class ParameterDescription
    {        
        public bool IsOptional;
    }

    [NetSerializable]
    public abstract class ParameterData
    {
        public int ParamIndex;
    }

    public sealed class UseContract
    {
        public ParameterDescription[] ParameterTypes;

        public UseContract(params ParameterDescription[] parameterTypes)
        {
            ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));
        }
    }

    public sealed class UseData
    {
        public ParameterData[] ParameterDatas;

        private UseData(params ParameterData[] parameterDatas)
        {
            ParameterDatas = parameterDatas ?? throw new ArgumentNullException(nameof(parameterDatas));
        }

        // using this instead with a private constructor will allow us to later use pooling without changing much code
        public static UseData Create(params ParameterData[] parameterDescription)
        {
            return new UseData(parameterDescription);
        }

        public bool TryGetParameter<T>(int index, out T parameterData) where T : ParameterData
        {
            for (int i = 0; i < ParameterDatas.Length; i++)
            {
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

    public abstract void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData);
    public abstract bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn);
}
