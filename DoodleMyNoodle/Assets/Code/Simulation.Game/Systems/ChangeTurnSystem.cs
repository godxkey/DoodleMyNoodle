using Unity.Entities;

public struct NeverEndingTurnTag : IComponentData { }

public struct NewTurnEventData : IComponentData { }

public struct RequestChangeTurnData : IComponentData
{
    public int TeamToPlay;
}

public struct TurnCountSingletonComponent : IComponentData
{
    public int Value;
}

public struct TurnTeamCountSingletonComponent : IComponentData
{
    public int Value;
}

public struct TurnTimerSingletonComponent : IComponentData
{
    public fix Value;
}

public struct TurnCurrentTeamSingletonComponent : IComponentData
{
    public int Value;
}

public struct TurnDurationSingletonComponent : IComponentData
{
    public fix DurationAI;
    public fix DurationPlayer;
}

public struct ReadyForNextTurn : IComponentData
{
    public bool Value;

    public static implicit operator bool(ReadyForNextTurn val) => val.Value;
    public static implicit operator ReadyForNextTurn(bool val) => new ReadyForNextTurn() { Value = val };
}


public class ChangeTurnSystem : SimSystemBase
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(NewTurnEventData));

        RequireSingletonForUpdate<TurnTimerSingletonComponent>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _eventsEntityQuery.Dispose();
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        // NB: We do that first to make sure we don't override a changeTurnRequest when we update the turn timer
        ChangeTurnIfRequested();

        if (!HasSingleton<NeverEndingTurnTag>())
        {
            UpdateTurnTimer();
        }
    }

    private void ChangeTurnIfRequested()
    {
        if (TryGetSingleton(out RequestChangeTurnData requestData))
        {
            DestroySingleton<RequestChangeTurnData>();

            // wrap team if necessary
            if (requestData.TeamToPlay >= GetSingleton<TurnTeamCountSingletonComponent>().Value)
            {
                requestData.TeamToPlay = 0;
            }

            // set new turn
            SetSingleton(new TurnCurrentTeamSingletonComponent { Value = requestData.TeamToPlay });
            SetSingleton(new TurnCountSingletonComponent { Value = GetSingleton<TurnCountSingletonComponent>().Value + 1 });

            switch ((DesignerFriendlyTeam)requestData.TeamToPlay)
            {
                case DesignerFriendlyTeam.Player:
                    SetSingleton(new TurnTimerSingletonComponent { Value = GetSingleton<TurnDurationSingletonComponent>().DurationPlayer });
                    break;
                case DesignerFriendlyTeam.Baddies:
                    SetSingleton(new TurnTimerSingletonComponent { Value = GetSingleton<TurnDurationSingletonComponent>().DurationAI });
                    break;
                default:
                    break;
            }

            // mark all 'ready' entities as 'unready'
            Entities.ForEach((ref ReadyForNextTurn readyForNextTurn) =>
            {
                readyForNextTurn.Value = false;
            }).Run();

            // fire event
            EntityManager.CreateEventEntity<NewTurnEventData>();
        }
    }

    private void UpdateTurnTimer()
    {
        if (TryGetSingleton(out TurnCurrentTeamSingletonComponent currentTeam))
        {
            if (currentTeam.Value == -1) // no team, no change turn
                return;
        }

        TurnTimerSingletonComponent turnTimer = GetOrCreateSingleton<TurnTimerSingletonComponent>();

        fix newTimerValue = turnTimer.Value - Time.DeltaTime;
        if (newTimerValue <= 0)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
        else
        {
            SetSingleton(new TurnTimerSingletonComponent { Value = newTimerValue });
        }
    }
}

public static partial class CommonReads
{
    public static int GetTurn(ISimWorldReadAccessor accessor)
    {
        if (accessor.TryGetSingleton(out TurnCountSingletonComponent v))
            return v.Value;
        return -1;
    }

    public static Team GetTurnTeam(ISimWorldReadAccessor accessor)
    {
        if (accessor.TryGetSingleton(out TurnCurrentTeamSingletonComponent v))
            return v.Value;
        return -1;
    }

    public static bool CanTeamPlay(ISimWorldReadAccessor accessor, Team team)
    {
        return GetTurnTeam(accessor) == team.Value;
    }
}

internal static partial class CommonWrites
{
    public static void RequestNextTurn(ISimWorldReadWriteAccessor accessor)
    {
        TurnCurrentTeamSingletonComponent turnCurrentTeam = accessor.GetSingleton<TurnCurrentTeamSingletonComponent>();

        int newCurrentTeam = turnCurrentTeam.Value + 1;

        RequestSetTurn(accessor, newCurrentTeam);
    }

    public static void RequestSetTurn(ISimWorldReadWriteAccessor accessor, int team)
    {
        accessor.SetOrCreateSingleton(new RequestChangeTurnData()
        {
            TeamToPlay = team
        });
    }
}
