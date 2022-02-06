using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Collections;

public partial class CommonReads
{
    public static Action.UseContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionEntity, Entity actionPrefab)
    {
        return GetActionContext(accessor, actionEntity, actionPrefab, targets : default);
    }

    public static Action.UseContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionEntity, Entity actionPrefab, Entity target)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        return GetActionContext(accessor, actionEntity, actionPrefab, targets);
    }

    public static Action.UseContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionEntity, Entity actionPrefab, NativeArray<Entity> targets)
    {
        Entity instigatorPawn = Entity.Null;
        if (accessor.TryGetComponent(actionEntity, out OwnerPawn ownerPawn))
        {
            instigatorPawn = ownerPawn.Value;
        }

        Action.UseContext useContext = new Action.UseContext()
        {
            ActionPrefab = actionPrefab,
            InstigatorPawn = instigatorPawn,
            ActionInstigator = actionEntity,
            Targets = targets
        };

        return useContext;
    }
}

internal partial class CommonWrites
{
    public static void ExecuteAction(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity action, Action.UseParameters parameters = null)
    {
        ExecuteAction(accessor, instigator, action, targets: default, parameters);
    }

    public static void ExecuteAction(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity action, Entity target, Action.UseParameters parameters = null)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        ExecuteAction(accessor, instigator, action, targets, parameters);
    }

    public static void ExecuteAction(ISimWorldReadWriteAccessor accessor, Entity actionEntity, Entity actionPrefab, NativeArray<Entity> targets, Action.UseParameters parameters = null)
    {
        if (!accessor.TryGetComponent(actionPrefab, out ActionId actionId) && actionId.IsValid)
            return;

        Action action = ActionBank.GetAction(actionId);

        if (!action.TryUse(accessor, CommonReads.GetActionContext(accessor, actionEntity, actionPrefab, targets), parameters, out string debugReason))
        {
            Log.Info($"Can't Trigger {action} because: {debugReason}");
            return;
        }
    }
}

public struct ActionUsedEventData
{
    public Action.UseContext ActionContext;
    public Action.ResultData ActionResult;
}

public abstract class Action
{
    public static LogChannel LogChannel = Log.CreateChannel("Actions", activeByDefault: true);

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
                    Log.Warning($"Action parameters[{index}] is null");
                parameterData = null;
                return false;
            }

            if (!(ParameterDatas[index] is T p))
            {
                if (warnIfFailed)
                    Log.Warning($"Action parameters[{index}] is of type {ParameterDatas[index].GetType().GetPrettyFullName()}," +
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
        public Entity ActionPrefab; // Prefab that contains the action and its informations
        public Entity InstigatorPawn; // origin pawn that owns this action
        public Entity ActionInstigator; // the entity triggering the action
        public NativeArray<Entity> Targets; // all the target for the action execution
    }

    public struct ResultData
    {
        // temp fix because we can't use list in events
        // for now, we can't have more than 4 consecutive action result/animation
        public ResultDataElement DataElement_0;
        public ResultDataElement DataElement_1;
        public ResultDataElement DataElement_2;
        public ResultDataElement DataElement_3;
        public int Count;
    }

    public struct ResultDataElement
    {
        public fix2 AttackVector;
        public fix2 Position;
        public Entity Entity;
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

        List<ResultDataElement> resultData = new List<ResultDataElement>();
        if (Use(accessor, context, parameters, resultData))
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
        int minApCost = GetMinimumActionPointCost(accessor, context);
        if (minApCost > 0)
        {
            if (!accessor.TryGetComponent(context.InstigatorPawn, out ActionPoints ap))
            {
                debugReason?.Set("Pawn has no ActionPoints component.");
                return false;
            }

            if (ap <= (fix)0)
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
        if (accessor.TryGetComponent(context.ActionInstigator, out ItemCooldownTimeCounter timeCooldown) &&
            timeCooldown.Value > 0)
        {
            return true;
        }

        if (accessor.TryGetComponent(context.ActionInstigator, out ItemCooldownTurnCounter turnCooldown) &&
            turnCooldown.Value > 0)
        {
            return true;
        }

        return false;
    }

    public void BeforeActionUsed(ISimWorldReadWriteAccessor accessor, in UseContext context)
    {

    }

    public void OnActionUsed(ISimWorldReadWriteAccessor accessor, in UseContext context, List<ResultDataElement> result)
    {
        // reduce consumable amount
        if (accessor.GetComponent<StackableFlag>(context.ActionInstigator))
        {
            CommonWrites.DecrementItem(accessor, context.ActionInstigator, context.ActionInstigator);
        }

        // reduce instigator AP
        if (accessor.TryGetComponent(context.InstigatorPawn, out ActionSettingAPCost itemActionPointCost))
        {
            CommonWrites.ModifyStatFix<ActionPoints>(accessor, context.ActionInstigator, -itemActionPointCost.Value);
        }

        // Cooldown
        if (accessor.TryGetComponent(context.ActionInstigator, out ItemTimeCooldownData itemTimeCooldownData))
        {
            accessor.SetOrAddComponent(context.ActionInstigator, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
        }
        else if (accessor.TryGetComponent(context.ActionInstigator, out ItemTurnCooldownData itemTurnCooldownData))
        {
            accessor.SetOrAddComponent(context.ActionInstigator, new ItemCooldownTurnCounter() { Value = itemTurnCooldownData.Value });
        }

        // Feedbacks
        ResultData resultData = new ResultData() { Count = result.Count };

        for (int i = 0; i < result.Count; i++)
        {
            switch (i)
            {
                case 0:
                    resultData.DataElement_0 = result[0];
                    break;
                case 1:
                    resultData.DataElement_1 = result[1];
                    break;
                case 2:
                    resultData.DataElement_2 = result[2];
                    break;
                case 3:
                    resultData.DataElement_3 = result[3];
                    break;
                default:
                    break;
            }
        }

        accessor.GetOrCreateSystem<PresentationEventSystem>().PresentationEvents.ActionEvents.Push(new ActionUsedEventData()
        {
            ActionContext = context,// todo: copy array and dispose in presentation
            ActionResult = resultData
        });
    }

    protected virtual int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponent(context.ActionInstigator, out ActionSettingAPCost apCost))
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

    public abstract bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData);
    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity actionPrefab);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogActionInfo(UseContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - {GetType().Name} - context(actionEntity: {context.ActionInstigator}, instigator: {context.InstigatorPawn})");
    }
}

public abstract class Action<TSetting> : Action where TSetting : struct, IComponentData
{
    public override Type[] GetRequiredSettingTypes() => new[] { typeof(TSetting) };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity actionPrefab)
    {
        var settings = accessor.GetComponent<TSetting>(actionPrefab);
        return GetUseContract(accessor, settings);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData)
    {
        var settings = accessor.GetComponent<TSetting>(context.ActionPrefab);
        return Use(accessor, context, parameters, resultData, settings);
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        var settings = accessor.GetComponent<TSetting>(context.ActionPrefab);
        return CanBeUsedInContextSpecific(accessor, context, debugReason, settings);
    }

    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, TSetting settings);
    public abstract bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, TSetting settings);
    protected virtual bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason, TSetting settings)
    {
        return true;
    }
}