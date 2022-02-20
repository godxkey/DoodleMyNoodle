using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Collections;

public partial class CommonReads
{
    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionInstigator, Entity actionEntity)
    {
        return GetActionContext(accessor, actionInstigator, actionEntity, targets: default);
    }

    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionInstigator, Entity actionEntity, Entity target)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        return GetActionContext(accessor, actionInstigator, actionEntity, targets);
    }

    public static GameAction.ExecutionContext GetActionContext(ISimWorldReadAccessor accessor, Entity actionInstigator, Entity actionEntity, NativeArray<Entity> targets)
    {
        Entity firstPhysicalInstigator = actionInstigator;
        if (accessor.TryGetComponent(actionInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        Entity lastPhysicalInstigator = firstPhysicalInstigator;
        if (lastPhysicalInstigator != actionInstigator && accessor.HasComponent<FixTranslation>(actionInstigator))
        {
            lastPhysicalInstigator = actionInstigator;
        }

        GameAction.ExecutionContext useContext = new GameAction.ExecutionContext()
        {
            Action = actionEntity,
            FirstPhysicalInstigator = firstPhysicalInstigator,
            LastPhysicalInstigator = lastPhysicalInstigator,
            ActionInstigatorActor = actionInstigator,
            Targets = targets,
        };

        return useContext;
    }
}

internal partial class CommonWrites
{
    public static void RequestExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity actionInstigator, Entity actionEntity, GameAction.UseParameters parameters = null)
    {
        RequestExecuteGameAction(accessor, actionInstigator, actionEntity, targets: default, parameters);
    }

    public static void RequestExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity actionInstigator, Entity actionEntity, Entity target, GameAction.UseParameters parameters = null)
    {
        var targets = new NativeArray<Entity>(1, Allocator.Temp);
        targets[0] = target;
        RequestExecuteGameAction(accessor, actionInstigator, actionEntity, targets, parameters);
    }

    public static void RequestExecuteGameAction(ISimGameWorldReadWriteAccessor accessor, Entity actionInstigator, Entity actionEntity, NativeArray<Entity> targets, GameAction.UseParameters parameters = null)
    {
        var system = accessor.GetExistingSystem<ExecuteGameActionSystem>();

        system.ActionRequestsManaged.Add(new ExecuteGameActionSystem.ActionRequestManaged()
        {
            ActionEntity = actionEntity,
            Instigator = actionInstigator,
            Parameters = parameters,
            Targets = targets
        });
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
        /// Very first instigator in chain of action with a physical location component (FixTranslation). This is generally the pawn that used the first action. Should never be null.
        /// (e.g. A pawn throws a poison arrow, and the arrow poisons a target. The arrow is the action instigator, but the pawn is the 'first instigator')
        /// </summary>
        public Entity FirstPhysicalInstigator;

        /// <summary>
        /// The last actor with a physical location component (FixTranslation). Should never be null.
        /// </summary>
        public Entity LastPhysicalInstigator;

        /// <summary>
        /// The actor triggering the action, should never be null. This could be a pawn, an item, a projectile, etc.
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

    public interface IExecuteAccessor
    {
        bool ExecuteGameAction(Entity actionInstigator, Entity actionEntity, Entity target);
        bool ExecuteGameAction(Entity actionInstigator, Entity actionEntity, NativeArray<Entity> targets);
    }

    public readonly struct ExecInputs
    {
        public readonly ISimGameWorldReadWriteAccessor Accessor;
        public readonly ExecutionContext Context;
        public readonly UseParameters Parameters;

        public ExecInputs(ISimGameWorldReadWriteAccessor accessor, ExecutionContext context, UseParameters parameters)
        {
            Accessor = accessor;
            Context = context;
            Parameters = parameters;
        }
    }

    public ref struct ExecOutput
    {
        public List<ResultDataElement> ResultData;
    }

    public abstract bool Execute(in ExecInputs input, ref ExecOutput output);
    public abstract ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab);

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogActionInfo(ExecutionContext context, string message)
    {
        Log.Info(LogChannel, $"{message} - {GetType().Name} - context(actionEntity: {context.ActionInstigatorActor}, instigator: {context.FirstPhysicalInstigator})");
    }
}

public abstract class GameAction<TSetting> : GameAction where TSetting : struct, IComponentData
{
    public override Type[] GetRequiredSettingTypes() => new[] { typeof(TSetting) };

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab)
    {
        var settings = accessor.GetComponent<TSetting>(actionPrefab);
        return GetExecutionContract(accessor, ref settings);
    }

    public override bool Execute(in ExecInputs input, ref ExecOutput output)
    {
        var settings = input.Accessor.GetComponent<TSetting>(input.Context.Action);
        return Execute(in input, ref output, ref settings);
    }

    protected abstract ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref TSetting settings);
    protected abstract bool Execute(in ExecInputs input, ref ExecOutput output, ref TSetting settings);
}