using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngineX;

[UpdateAfter(typeof(InputSystemGroup))]
[AlwaysUpdateSystem]
public class ExecuteGameActionSystem : SimGameSystemBase
{
    public struct ActionRequest
    {
        public Entity Instigator;
        public Entity ActionEntity;

        /// <summary>
        /// Optional
        /// </summary>
        public Entity Target;
    }

    public struct ActionRequestManaged
    {
        public Entity Instigator;
        public Entity ActionEntity;

        /// <summary>
        /// Optional
        /// </summary>
        public NativeArray<Entity> Targets;

        /// <summary>
        /// Optional
        /// </summary>
        public GameAction.UseParameters Parameters;
    }

    public NativeList<ActionRequest> ActionRequests;
    public List<ActionRequestManaged> ActionRequestsManaged = new List<ActionRequestManaged>();
    public NativeList<JobHandle> HandlesToWaitFor;

    private NativeList<ActionRequest> _tmpActionRequests;
    private List<ActionRequestManaged> _tmpActionRequestsManaged = new List<ActionRequestManaged>();

    protected override void OnCreate()
    {
        base.OnCreate();
        ActionRequests = new NativeList<ActionRequest>(Allocator.Persistent);
        HandlesToWaitFor = new NativeList<JobHandle>(Allocator.Persistent);
        _tmpActionRequests = new NativeList<ActionRequest>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ActionRequests.Dispose();
        HandlesToWaitFor.Dispose();
        _tmpActionRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        JobHandle.CombineDependencies(HandlesToWaitFor.AsArray()).Complete();
        HandlesToWaitFor.Clear();

        bool isGameReadyForGameActions = HasSingleton<GridInfo>();

        while (ActionRequests.Length > 0 || ActionRequestsManaged.Count > 0)
        {
            _tmpActionRequests.CopyFrom(ActionRequests);
            _tmpActionRequestsManaged.Clear();
            _tmpActionRequestsManaged.AddRange(ActionRequestsManaged);
            ActionRequests.Clear();
            ActionRequestsManaged.Clear();

            if (isGameReadyForGameActions)
            {
                if (_tmpActionRequests.Length > 0)
                {
                    foreach (var request in _tmpActionRequests)
                    {
                        var targets = new NativeArray<Entity>(1, Allocator.Temp);
                        targets[0] = request.Target;
                        ExecuteGameAction(request.Instigator, request.ActionEntity, targets);
                    }
                }

                if (_tmpActionRequestsManaged.Count > 0)
                {
                    foreach (var request in _tmpActionRequestsManaged)
                    {
                        ExecuteGameAction(request.Instigator, request.ActionEntity, request.Targets, request.Parameters);
                        if (request.Targets.IsCreated)
                            request.Targets.Dispose();
                    }
                }
            }
        }
    }

    private bool ExecuteGameAction(Entity actionInstigator, Entity actionEntity, NativeArray<Entity> targets, GameAction.UseParameters parameters = null)
    {
        if (!TryGetComponent(actionEntity, out GameActionId actionId) || !actionId.IsValid)
        {
            Log.Error($"Could not find valid game action id on action {EntityManager.GetNameSafe(actionEntity)}. Action instigator: {EntityManager.GetNameSafe(actionInstigator)}");
            return false;
        }

        GameAction gameAction = GameActionBank.GetAction(actionId);

        if (gameAction == null)
            return false; // error is already logged in 'GetAction' method

        GameAction.ExecInputs input = new GameAction.ExecInputs(Accessor, CommonReads.GetActionContext(Accessor, actionInstigator, actionEntity, targets), parameters);
        GameAction.ExecOutput output = new GameAction.ExecOutput()
        {
            ResultData = new List<GameAction.ResultDataElement>()
        };

        if (!gameAction.Execute(in input, ref output))
        {
            // Feedbacks
            GameAction.ResultData resultData = new GameAction.ResultData() { Count = output.ResultData.Count };

            for (int i = 0; i < output.ResultData.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        resultData.DataElement_0 = output.ResultData[0];
                        break;
                    case 1:
                        resultData.DataElement_1 = output.ResultData[1];
                        break;
                    case 2:
                        resultData.DataElement_2 = output.ResultData[2];
                        break;
                    case 3:
                        resultData.DataElement_3 = output.ResultData[3];
                        break;
                    default:
                        break;
                }
            }

            PresentationEvents.GameActionEvents.Push(new GameActionUsedEventData()
            {
                GameActionContext = input.Context,// todo: copy array and dispose in presentation
                GameActionResult = resultData
            });

            Log.Info($"Couldn't use {gameAction}.");
            return false;
        }

        return true;
    }
}