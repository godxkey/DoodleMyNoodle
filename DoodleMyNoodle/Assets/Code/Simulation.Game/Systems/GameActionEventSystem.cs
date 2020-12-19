using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct GameActionEventData : IComponentData
{
    public Entity InstigatorPawn;
}

public struct GameActionEventRequestData : IBufferElementData
{
    public Entity InstigatorPawn;
}

public struct GameActionEventRequestSingletonTag : IComponentData { }

public class GameActionEventSystem : SimSystemBase
{
    private EntityQuery _gameActionEvents;

    private List<GameActionEventData> _newGameActionEvents = new List<GameActionEventData>();

    protected override void OnCreate()
    {
        base.OnCreate();

        _gameActionEvents = GetEntityQuery(typeof(GameActionEventData));
    }

    public void RequestGameActionEvent(GameActionEventRequestData gameActionRequestData)
    {
        GetGameActionEventRequestBuffer().Add(gameActionRequestData);
    }

    private DynamicBuffer<GameActionEventRequestData> GetGameActionEventRequestBuffer()
    {
        if (!HasSingleton<GameActionEventRequestSingletonTag>())
        {
            EntityManager.CreateEntity(typeof(GameActionEventRequestSingletonTag), typeof(GameActionEventRequestData));
        }

        return GetBuffer<GameActionEventRequestData>(GetSingletonEntity<GameActionEventRequestSingletonTag>());
    }

    protected override void OnUpdate()
    {
        // Clear Previously Applied Events
        EntityManager.DestroyEntity(_gameActionEvents);

        // Handle Event Request
        DynamicBuffer<GameActionEventRequestData> gameActionEventRequests = GetGameActionEventRequestBuffer();

        foreach (GameActionEventRequestData gameActionEventRequestData in gameActionEventRequests)
        {
            ProcessGameActionEventRequest(gameActionEventRequestData.InstigatorPawn, _newGameActionEvents);
        }

        gameActionEventRequests.Clear();

        // Handle Events
        foreach (GameActionEventData damageAppliedEventData in _newGameActionEvents)
        {
            EntityManager.CreateEventEntity(damageAppliedEventData);
        }

        _newGameActionEvents.Clear();
    }

    private void ProcessGameActionEventRequest(Entity instigator, List<GameActionEventData> outGameActionEvents)
    {
        outGameActionEvents.Add(new GameActionEventData()
        {
            InstigatorPawn = instigator
        });
    }
}

internal static partial class CommonWrites
{
    public static void RequestGameActionEvent(ISimWorldReadWriteAccessor accessor, GameAction.UseContext context, GameAction.ResultData result)
    {
        var gameActionRequest = new GameActionEventRequestData() { InstigatorPawn = context.InstigatorPawn };
        accessor.GetExistingSystem<GameActionEventSystem>().RequestGameActionEvent(gameActionRequest);
    }
}