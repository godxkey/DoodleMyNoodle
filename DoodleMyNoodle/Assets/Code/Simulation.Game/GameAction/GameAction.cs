using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;

public abstract class GameAction
{
    public static LogChannel LogChannel = Log.CreateChannel("Game Actions", activeByDefault: true);

    public virtual Type[] GetRequiredSettingTypes() => new Type[] { };

    public enum ParameterDescriptionType
    {
        None,
        Tile,
        Entity,
        SuccessRating,
        Vector,
        Position,
        Bool
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

        public bool TryGetParameter<T>(int index, out T parameterData, bool warnIfFailed = true) where T : ParameterData
        {
            if (index < 0 || index >= ParameterDatas.Length)
            {
                parameterData = null;
                return false;
            }

            if (ParameterDatas[index] is null)
            {
                if (warnIfFailed)
                    Log.Warning($"GameAction parameters[{index}] is null");
                parameterData = null;
                return false;
            }

            if (!(ParameterDatas[index] is T p))
            {
                if (warnIfFailed)
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
        public Entity Item;

        public UseContext(Entity instigatorPawnController, Entity instigatorPawn, Entity item)
        {
            InstigatorPawnController = instigatorPawnController;
            InstigatorPawn = instigatorPawn;
            Item = item;
        }
    }

    public struct ResultData
    {
        public fix2 AttackVector;
    }

    public class DebugReason
    {
        private string _reason = null;

        [Conditional("DEBUG")]
        public void Set(string reason) { _reason = reason; }
        public string Get() { return _reason; }
    }

    private static Pool<DebugReason> s_debugReasonPool = new Pool<DebugReason>();

    public bool TryUse(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, out string debugReason)
    {
        if (!CanBeUsedInContext(accessor, context, out debugReason))
        {
            return false;
        }

        BeforeActionUsed(accessor, context);

        ResultData resultData = new ResultData();
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
            if (!accessor.TryGetComponent(context.InstigatorPawn, out ActionPoints ap))
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
        if (accessor.TryGetComponent(context.Item, out ItemCooldownTimeCounter timeCooldown) &&
            timeCooldown.Value > 0)
        {
            return true;
        }

        if (accessor.TryGetComponent(context.Item, out ItemCooldownTurnCounter turnCooldown) &&
            turnCooldown.Value > 0)
        {
            return true;
        }

        return false;
    }

    public void BeforeActionUsed(ISimWorldReadWriteAccessor accessor, in UseContext context)
    {
        // hack can't move anymore after doing an action
        if (!(this is GameActionBasicJump))
        {
            accessor.SetComponent(context.InstigatorPawn, new MoveEnergy() { Value = 0 });
        }
    }

    public void OnActionUsed(ISimWorldReadWriteAccessor accessor, in UseContext context, ResultData result)
    {
        // reduce consumable amount
        if (accessor.GetComponent<StackableFlag>(context.Item))
        {
            CommonWrites.DecrementItem(accessor, context.Item, context.InstigatorPawn);
        }

        // reduce instigator AP
        if (accessor.TryGetComponent(context.Item, out GameActionSettingAPCost itemActionPointCost))
        {
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -itemActionPointCost.Value);
        }

        // Cooldown
        if (accessor.TryGetComponent(context.Item, out ItemTimeCooldownData itemTimeCooldownData))
        {
            accessor.SetOrAddComponent(context.Item, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
        }
        else if (accessor.TryGetComponent(context.Item, out ItemTurnCooldownData itemTurnCooldownData))
        {
            accessor.SetOrAddComponent(context.Item, new ItemCooldownTurnCounter() { Value = itemTurnCooldownData.Value });
        }

        // Feedbacks
        CommonWrites.RequestGameActionEvent(accessor, context, result);
    }

    protected virtual int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponent(context.Item, out GameActionSettingAPCost apCost))
        {
            return apCost.Value;
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
        Log.Info(LogChannel, $"{message} - {GetType().Name} - context(item: {context.Item}, instigator: {context.InstigatorPawn}, instigatorController: {context.InstigatorPawnController})");
    }
}

public abstract class GameAction<TSetting> : GameAction where TSetting : struct, IComponentData
{
    public override Type[] GetRequiredSettingTypes() => new[] { typeof(TSetting) };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var settings = accessor.GetComponent<TSetting>(context.Item);
        return GetUseContract(accessor, context, settings);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        var settings = accessor.GetComponent<TSetting>(context.Item);
        return Use(accessor, context, parameters, ref resultData, settings);
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        var settings = accessor.GetComponent<TSetting>(context.Item);
        return CanBeUsedInContextSpecific(accessor, context, debugReason, settings);
    }

    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, TSetting settings);
    public abstract bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, TSetting settings);
    protected virtual bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason, TSetting settings)
    {
        return true;
    }
}