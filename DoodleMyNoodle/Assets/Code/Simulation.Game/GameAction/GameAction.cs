using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Collections;

internal partial class CommonWrites
{
    public static void ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity instigator, Entity action, GameAction.UseParameters parameters = null)
    {
        ExecuteGameAction(accessor, instigator, action, targets: default, parameters);
    }

    public static void ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity instigator, Entity action, Entity target, GameAction.UseParameters parameters = null)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        ExecuteGameAction(accessor, instigator, action, targets, parameters);
    }

    public static void ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity instigator, Entity action, NativeArray<Entity> targets, GameAction.UseParameters parameters = null)
    {
        if (!accessor.TryGetComponent(action, out GameActionId gameActionId) && gameActionId.IsValid)
            return;

        GameAction gameAction = GameActionBank.GetAction(gameActionId);

        if (gameAction == null)
            return; // error is already logged in 'GetAction' method

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = instigator,
            Item = action,
            Targets = targets
        };

        if (!gameAction.TryUse(accessor, useContext, null, out string debugReason))
        {
            Log.Info($"Can't Trigger {gameAction} because: {debugReason}");
            return;
        }
    }
}

public struct GameActionUsedEventData
{
    public GameAction.UseContext GameActionContext;
    public GameAction.ResultData GameActionResult;
}

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
        public Entity InstigatorPawnController; // we can probably remove that later
        public Entity InstigatorPawn;
        public Entity Item;
        public NativeArray<Entity> Targets; // currently unused, but it should be!
    }

    public struct ResultData
    {
        // temp fix because we can't use list in events
        // for now, we can't have more than 4 consecutive game action result/animation
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

    public bool TryUse(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, out string debugReason)
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
        if (!accessor.HasComponent<FixTranslation>(context.InstigatorPawn))
        {
            debugReason?.Set("Pawn has no FixTranslation.");
            return false;
        }

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

        if (!accessor.HasComponent<GameActionBasicJump.Settings>(context.Item))
        {
            if (accessor.TryGetComponent(context.InstigatorPawn, out ItemUsedThisTurn itemUses) &&
                accessor.TryGetComponent(context.InstigatorPawn, out MaxItemUsesPerTurn maxUses))
            {
                if (itemUses >= maxUses)
                {
                    debugReason?.Set($"Already used {itemUses.Value} items this turn (max:{maxUses.Value})");
                    return false;
                }
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

    public void BeforeActionUsed(ISimGameWorldReadWriteAccessor accessor, in UseContext context)
    {

    }

    public void OnActionUsed(ISimGameWorldReadWriteAccessor accessor, in UseContext context, List<ResultDataElement> result)
    {
        // reduce consumable amount
        if (accessor.GetComponent<StackableFlag>(context.Item))
        {
            CommonWrites.DecrementItem(accessor, context.Item, context.InstigatorPawn);
        }

        // reduce instigator AP
        if (accessor.TryGetComponent(context.Item, out GameActionSettingAPCost itemActionPointCost))
        {
            CommonWrites.ModifyStatFix<ActionPoints>(accessor, context.InstigatorPawn, -itemActionPointCost.Value);
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

        // Item Used (hard coded to no count the basic jump)
        if (!accessor.HasComponent<GameActionBasicJump.Settings>(context.Item))
        {
            if (accessor.TryGetComponent(context.InstigatorPawn, out ItemUsedThisTurn itemUses))
            {
                accessor.SetComponent(context.InstigatorPawn, new ItemUsedThisTurn() { Value = itemUses + 1 });
            }
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

        accessor.PresentationEvents.GameActionEvents.Push(new GameActionUsedEventData()
        {
            GameActionContext = context,// todo: copy array and dispose in presentation
            GameActionResult = resultData
        });
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

    public abstract bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData);
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

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData)
    {
        var settings = accessor.GetComponent<TSetting>(context.Item);
        return Use(accessor, context, parameters, resultData, settings);
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        var settings = accessor.GetComponent<TSetting>(context.Item);
        return CanBeUsedInContextSpecific(accessor, context, debugReason, settings);
    }

    public abstract UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, TSetting settings);
    public abstract bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, TSetting settings);
    protected virtual bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason, TSetting settings)
    {
        return true;
    }
}