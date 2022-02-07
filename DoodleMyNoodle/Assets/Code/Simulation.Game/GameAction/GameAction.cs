using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Collections;

public partial class CommonReads
{
    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity instigator, Entity actionPrefab)
    {
        return GetActionContext(accessor, instigator, actionPrefab, targets: default);
    }

    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity instigator, Entity actionPrefab, Entity target)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        return GetActionContext(accessor, instigator, actionPrefab, targets);
    }

    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity instigator, Entity actionPrefab, NativeArray<Entity> targets)
    {
        Entity firstInstigator = instigator;
        if (accessor.TryGetComponent(instigator, out FirstInstigator firstInstigatorComponent))
        {
            firstInstigator = firstInstigatorComponent.Value;
        }

        GameAction.ExecutionContext useContext = new GameAction.ExecutionContext()
        {
            Action = actionPrefab,
            FirstInstigatorActor = firstInstigator,
            ActionInstigatorActor = instigator,
            Targets = targets
        };

        return useContext;
    }
}

internal partial class CommonWrites
{
    public static bool ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity instigator, Entity action, GameAction.UseParameters parameters = null)
    {
        return ExecuteGameAction(accessor, instigator, action, targets: default, parameters);
    }

    public static bool ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity instigator, Entity action, Entity target, GameAction.UseParameters parameters = null)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        return ExecuteGameAction(accessor, instigator, action, targets, parameters);
    }

    public static bool ExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity actionEntity, Entity actionPrefab, NativeArray<Entity> targets, GameAction.UseParameters parameters = null)
    {
        if (!accessor.TryGetComponent(actionPrefab, out GameActionId actionId) && actionId.IsValid)
            return false;

        GameAction gameAction = GameActionBank.GetAction(actionId);

        if (gameAction == null)
            return false; // error is already logged in 'GetAction' method

        if (!gameAction.TryExecute(accessor, CommonReads.GetActionContext(accessor, actionEntity, actionPrefab, targets), parameters))
        {
            Log.Info($"Couldn't use {gameAction}.");
            return false;
        }

        return true;
    }
}

public struct GameActionUsedEventData
{
    public GameAction.ExecutionContext GameActionContext;
    public GameAction.ResultData GameActionResult;
}

public abstract class GameAction
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

    public sealed class ExecutionContract
    {
        public ParameterDescription[] ParameterTypes;

        public ExecutionContract(params ParameterDescription[] parameterTypes)
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

    public struct ExecutionContext
    {
        /// <summary>
        /// Prefab that contains the action and its informations
        /// </summary>
        public Entity Action;

        /// <summary>
        /// Very first instigator in chain of action. This is generally the pawn that used the first action. 
        /// (e.g. A pawn throws a poison arrow, and the arrow poisons a target. The arrow is the action instigator, but the pawn is the 'first instigator')
        /// </summary>
        public Entity FirstInstigatorActor;

        /// <summary>
        /// The actor triggering the action, should never be null. 
        /// </summary>
        public Entity ActionInstigatorActor;

        /// <summary>
        /// All the target for the action execution, could be empty or uncreated
        /// </summary>
        public NativeArray<Entity> Targets;
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

    public bool TryExecute(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters parameters)
    {
        List<ResultDataElement> result = new List<ResultDataElement>();
        if (Execute(accessor, context, parameters, result))
        {
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

            accessor.GetOrCreateSystem<PresentationEventSystem>().PresentationEvents.GameActionEvents.Push(new GameActionUsedEventData()
            {
                GameActionContext = context,// todo: copy array and dispose in presentation
                GameActionResult = resultData
            });

            return true;
        }
        else
        {
            return false;
        }
    }

    public abstract bool Execute(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters parameters, List<ResultDataElement> resultData);
    public abstract ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogActionInfo(ExecutionContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - {GetType().Name} - context(actionEntity: {context.ActionInstigatorActor}, instigator: {context.FirstInstigatorActor})");
    }
}

public abstract class GameAction<TSetting> : GameAction where TSetting : struct, IComponentData
{
    public override Type[] GetRequiredSettingTypes() => new[] { typeof(TSetting) };

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab)
    {
        var settings = accessor.GetComponent<TSetting>(actionPrefab);
        return GetExecutionContract(accessor, settings);
    }

    public override bool Execute(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters parameters, List<ResultDataElement> resultData)
    {
        var settings = accessor.GetComponent<TSetting>(context.Action);
        return Use(accessor, context, parameters, resultData, settings);
    }

    public abstract ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, TSetting settings);
    public abstract bool Use(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters parameters, List<ResultDataElement> resultData, TSetting settings);
}