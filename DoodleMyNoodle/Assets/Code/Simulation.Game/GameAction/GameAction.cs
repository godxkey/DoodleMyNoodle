using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
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
                    Log.Warning($"GameAction parameters[{i}] is null");
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

        public UseContext(Entity instigatorPawnController, Entity instigatorPawn, Entity entity)
        {
            InstigatorPawnController = instigatorPawnController;
            InstigatorPawn = instigatorPawn;
            Entity = entity;
        }
    }

    public class DebugReason
    {
        private string _reason = null;

        [Conditional("DEBUG")]
        public void Set(string reason) { _reason = reason; }
        public string Get() { return _reason; }
    }

    private static Pool<DebugReason> s_debugReasonPool = new Pool<DebugReason>();

    public bool TryUse(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (!CanBeUsedInContext(accessor, context))
        {
            return false;
        }

        Use(accessor, context, parameters);
        return true;
    }

    public bool TryUse(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, out string debugReason)
    {
        if (!CanBeUsedInContext(accessor, context, out debugReason))
        {
            return false;
        }

        Use(accessor, context, parameters);
        return true;
    }

    public bool CanBeUsedInContext(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return CanBeUsedInContextInternal(accessor, context, null);
    }

    public bool CanBeUsedInContext(ISimWorldReadAccessor accessor, in UseContext context, out string debugReason)
    {
        DebugReason reason = s_debugReasonPool.Take();

        bool result = CanBeUsedInContextInternal(accessor, context, reason);

        debugReason = reason.Get();

        s_debugReasonPool.Release(reason);

        return result;
    }

    private bool CanBeUsedInContextInternal(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        if (!accessor.HasComponent<FixTranslation>(context.InstigatorPawn))
        {
            debugReason?.Set("Pawn has no FixTranslation.");
            return false;
        }

        int minApCost = GetMinimumActionPointCost(accessor, context);
        if (minApCost > 0)
        {
            if (!accessor.TryGetComponentData(context.InstigatorPawn, out ActionPoints ap))
            {
                debugReason?.Set("Pawn has no ActionPoints component.");
                return false;
            }

            if (ap < minApCost)
            {
                debugReason?.Set($"Pawn doesn't have enough ActionPoints (has {ap.Value}, need {minApCost}).");
                return false;
            }
        }

        if (IsInCooldown(accessor, context))
        {
            debugReason?.Set("In cooldown");
            return false;
        }

        return CanBeUsedInContextSpecific(accessor, context, debugReason);
    }

    public bool IsInCooldown(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponentData(context.Entity, out ItemCooldownTimeCounter timeCooldown) &&
            timeCooldown.Value > 0)
        {
            return true;
        }

        if (accessor.TryGetComponentData(context.Entity, out ItemCooldownTurnCounter turnCooldown) &&
            turnCooldown.Value > 0)
        {
            return true;
        }

        return false;
    }

    protected abstract int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context);
    protected abstract bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason);
    public abstract void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogGameActionInfo(in UseContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - context(item: {context.Entity}, instigator: {context.InstigatorPawn}, instigatorController: {context.InstigatorPawnController})");
    }
}