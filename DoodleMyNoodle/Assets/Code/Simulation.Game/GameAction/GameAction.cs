using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;

public abstract class GameAction
{
    public static LogChannel LogChannel = Log.CreateChannel("Game Actions", activeByDefault: true);

    public enum ParameterDescriptionType
    {
        None,
        Tile,
        Entity,
        SuccessRate,
        Vector,
        Position
    }

    public abstract class ParameterDescription
    {
        public abstract ParameterDescriptionType GetParameterDescriptionType();
    }

    [NetSerializable(IsBaseClass = true)]
    public abstract class ParameterData
    {
        public ParameterData() { }
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
            if (index < 0 || index >= ParameterDatas.Length)
            {
                parameterData = null;
                return false;
            }

            if (ParameterDatas[index] is null)
            {
                Log.Warning($"GameAction parameters[{index}] is null");
                parameterData = null;
                return false;
            }

            if (!(ParameterDatas[index] is T p))
            {
                Log.Warning($"GameAction parameters[{index}] is of type {ParameterDatas[index].GetType().GetPrettyFullName()}," +
                    $" not of expected type {typeof(T).GetPrettyFullName()}");
                parameterData = null;
                return false;
            }

            parameterData = p;
            return true;
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

    public struct ResultData
    {
        public Dictionary<string, object> Data;

        public ResultData(Dictionary<string, object> defaultData) 
        {
            Data = defaultData;
        }

        public void AddData(params KeyValuePair<string, object>[] data)
        {
            foreach (KeyValuePair<string, object> item in data)
            {
                Data.Add(item.Key, item.Value);
            }
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

        ResultData resultData = new ResultData(new Dictionary<string, object>());
        if (Use(accessor, context, parameters, ref resultData))
        {
            OnActionUsed(accessor, context, resultData);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryUse(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, out string debugReason)
    {
        if (!CanBeUsedInContext(accessor, context, out debugReason))
        {
            return false;
        }

        ResultData resultData = new ResultData(new Dictionary<string, object>());
        if (Use(accessor, context, parameters, ref resultData))
        {
            OnActionUsed(accessor, context, resultData);
            return true;
        }
        else
        {
            return false;
        }
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
        if (minApCost >= 0)
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

    public void OnActionUsed(ISimWorldReadWriteAccessor accessor, in UseContext context, ResultData result)
    {
        // reduce consumable amount
        if (accessor.TryGetComponentData(context.Entity, out ItemStackableData itemStacked))
        {
            CommonWrites.DecrementStackableItemInInventory(accessor, context.InstigatorPawn, context.Entity);
        }

        // reduce instigator AP
        if (accessor.TryGetComponentData(context.Entity, out GameActionAPCostData itemActionPointCost))
        {
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -itemActionPointCost.Value);
        }

        // increase instigator AP
        if (accessor.TryGetComponentData(context.Entity, out GameActionAPGainData itemActionPointGain))
        {
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, itemActionPointGain.Value);
        }

        // reduce instigator Health
        if (accessor.TryGetComponentData(context.Entity, out GameActionHPCostData itemHealthPointCost))
        {
            CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, context.InstigatorPawn, itemHealthPointCost.Value);
        }

        // Cooldown
        if (accessor.TryGetComponentData(context.Entity, out ItemTimeCooldownData itemTimeCooldownData))
        {
            accessor.SetOrAddComponentData(context.Entity, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
        }
        else if (accessor.TryGetComponentData(context.Entity, out ItemTurnCooldownData itemTurnCooldownData))
        {
            accessor.SetOrAddComponentData(context.Entity, new ItemCooldownTurnCounter() { Value = itemTurnCooldownData.Value });
        }

        // Feedbacks
        GameActionPresentationFeedbacks.OnGameActionUsed(accessor, context, result);
        CommonWrites.RequestGameActionEvent(accessor, context, result);
    }

    protected virtual int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponentData(context.Entity, out GameActionAPCostData ActionPointCost))
        {
            return ActionPointCost.Value;
        }
        else
        {
            return 0;
        }
    }

    protected virtual bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason) 
    {
        return true;
    }

    public abstract bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogGameActionInfo(UseContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - {GetType().Name} - context(item: {context.Entity}, instigator: {context.InstigatorPawn}, instigatorController: {context.InstigatorPawnController})");
    }
}