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
    }

    public sealed class UseData
    {
        public ParameterData[] ParameterDatas;

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

    public abstract void Use(ISimWorldReadWriteAccessor accessor, Entity instigator, UseData useData);
    public abstract bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigator);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigator);
}
