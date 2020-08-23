using Unity.Entities;

public struct NewTurnEventData : IComponentData
{
}

public struct RequestChangeTurnData : IComponentData
{
    public int TeamToPlay;
}

public class ChangeTurnSystem : SimComponentSystem
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(NewTurnEventData));

        RequireSingletonForUpdate<TurnTimer>();
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

        UpdateTurnTimer();
    }

    private void ChangeTurnIfRequested()
    {
        if (this.TryGetSingleton(out RequestChangeTurnData requestData))
        {
            this.DestroySingleton<RequestChangeTurnData>();

            // wrap team if necessary
            if (requestData.TeamToPlay >= GetSingleton<TurnTeamCount>().Value)
            {
                requestData.TeamToPlay = 0;
            }

            // set new turn
            SetSingleton(new TurnCurrentTeam { Value = requestData.TeamToPlay });

            switch ((DesignerFriendlyTeam)requestData.TeamToPlay)
            {
                case DesignerFriendlyTeam.Player:
                    SetSingleton(new TurnTimer { Value = GetSingleton<TurnDuration>().DurationPlayer });
                    break;
                case DesignerFriendlyTeam.Baddies:
                    SetSingleton(new TurnTimer { Value = GetSingleton<TurnDuration>().DurationAI });
                    break;
                default:
                    break;
            }

            // mark all 'ready' entities as 'unready'
            Entities.ForEach((ref ReadyForNextTurn readyForNextTurn) =>
            {
                readyForNextTurn.Value = false;
            });

            // fire event
            EntityManager.CreateEventEntity<NewTurnEventData>();
        }
    }

    private void UpdateTurnTimer()
    {
        if(this.TryGetSingleton(out TurnCurrentTeam currentTeam))
        {
            if (currentTeam.Value == -1) // no team, no change turn
                return;
        }


        TurnTimer turnTimer = this.GetOrCreateSingleton<TurnTimer>();

        fix newTimerValue = turnTimer.Value - Time.DeltaTime;
        if (newTimerValue <= 0)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
        else
        {
            SetSingleton(new TurnTimer { Value = newTimerValue });
        }
    }
}

public static partial class CommonReads
{
    public static int GetCurrentTurnTeam(ISimWorldReadAccessor accessor)
    {
        if (accessor.TryGetSingleton(out TurnCurrentTeam v))
            return v.Value;
        return -1;
    }

    public static bool CanTeamPlay(ISimWorldReadAccessor accessor, Team team)
    {
        return GetCurrentTurnTeam(accessor) == team.Value;
    }
}

internal static partial class CommonWrites
{
    public static void RequestNextTurn(ISimWorldReadWriteAccessor accessor)
    {
        TurnCurrentTeam turnCurrentTeam = accessor.GetSingleton<TurnCurrentTeam>();

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
