using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngineX;

public struct GameActionRequest
{
    public Entity Instigator;
    public Entity ActionEntity;

    /// <summary>
    /// Optional
    /// </summary>
    public Entity Target;
}

public struct GameActionRequestManaged
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

[UpdateAfter(typeof(InputSystemGroup))]
[AlwaysUpdateSystem]
public class ExecuteGameActionSystem : SimGameSystemBase
{
    private List<NativeList<GameActionRequest>> _actionRequests = new List<NativeList<GameActionRequest>>();
    private List<GameActionRequestManaged> _actionRequestsManaged = new List<GameActionRequestManaged>();
    private NativeList<JobHandle> _handlesToWaitFor;
    private NativeList<GameActionRequest> _processingRequests;
    private List<GameActionRequestManaged> _processingRequestsManaged = new List<GameActionRequestManaged>();

    public List<GameActionRequestManaged> ActionRequestsManaged => _actionRequestsManaged;
    public NativeList<JobHandle> HandlesToWaitFor => _handlesToWaitFor;
    public List<NativeList<GameActionRequest>> InternalGetRequestBufferList => _actionRequests;

    public NativeList<GameActionRequest> CreateRequestBuffer()
    {
        var result = new NativeList<GameActionRequest>(Allocator.TempJob);
        _actionRequests.Add(result);
        return result;
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        _handlesToWaitFor = new NativeList<JobHandle>(Allocator.Persistent);
        _processingRequests = new NativeList<GameActionRequest>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _handlesToWaitFor.Dispose();
        _processingRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        JobHandle.CombineDependencies(_handlesToWaitFor.AsArray()).Complete();
        _handlesToWaitFor.Clear();

        bool isGameReadyForGameActions = HasSingleton<GridInfo>();

        while (_actionRequests.Count > 0 || _actionRequestsManaged.Count > 0)
        {
            _processingRequests.Clear();
            for (int i = 0; i < _actionRequests.Count; i++)
            {
                _processingRequests.AddRange(_actionRequests[i]);
                _actionRequests[i].Dispose();
            }
            _actionRequests.Clear();

            _processingRequestsManaged.Clear();
            _processingRequestsManaged.AddRange(_actionRequestsManaged);
            _actionRequestsManaged.Clear();


            if (isGameReadyForGameActions)
            {
                if (_processingRequests.Length > 0)
                {
                    foreach (var request in _processingRequests)
                    {
                        var targets = new NativeArray<Entity>(1, Allocator.Temp);
                        targets[0] = request.Target;
                        ExecuteGameAction(request.Instigator, request.ActionEntity, targets);
                    }
                }

                if (_processingRequestsManaged.Count > 0)
                {
                    foreach (var request in _processingRequestsManaged)
                    {
                        ExecuteGameAction(request.Instigator, request.ActionEntity, request.Targets, request.Parameters);
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
            Log.Info($"Couldn't use {gameAction}.");
            return false;
        }

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


        // copy native array to make sure its persisten
        var persistentContext = input.Context;

        if (input.Context.Targets.IsCreated)
        {
            persistentContext.Targets = new NativeArray<Entity>(input.Context.Targets.Length, Allocator.Persistent);
            input.Context.Targets.CopyTo(persistentContext.Targets);
        }

        PresentationEvents.GameActionEvents.Push(new GameActionUsedEventData()
        {
            GameActionContext = persistentContext,
            GameActionResult = resultData
        });

        return true;
    }
}