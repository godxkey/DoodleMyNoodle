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
        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(NewTurnEventData));
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        // NB: We do that first to make sure we don't override a changeTurnRequest when we update the turn timer
        ChangeTurnIfRequested();

        UpdateTurnTimer();
    }

    private void ChangeTurnIfRequested()
    {
        //EntityManager.DestroyEntity()

        if (this.TryGetSingleton(out RequestChangeTurnData requestData))
        {
            // wrap team if necessary
            if (requestData.TeamToPlay >= GetSingleton<TurnTeamCount>().Value)
            {
                requestData.TeamToPlay = 0;
            }

            // set new turn
            SetSingleton(new TurnCurrentTeam { Value = requestData.TeamToPlay });
            SetSingleton(new TurnTimer { Value = GetSingleton<TurnDuration>().Value });

            // fire event
            EntityManager.CreateEventEntity<NewTurnEventData>();

            this.DestroySingleton<RequestChangeTurnData>();
        }
    }

    private void UpdateTurnTimer()
    {
        TurnTimer turnTimer = GetSingleton<TurnTimer>();

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
        return accessor.GetSingleton<TurnCurrentTeam>().Value;
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
