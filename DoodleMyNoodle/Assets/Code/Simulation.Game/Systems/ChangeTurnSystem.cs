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


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ChangeTurnSystem : SimGameSystemBase
{
    private EntityQuery _eventsEntityQuery;

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

        ChangeTurnIfRequested();

        if (!HasSingleton<NeverEndingTurnTag>())
        {
            UpdateTurnTimer();
        }
    }

    private void ChangeTurnIfRequested()
    {
        // NB: We do that first to make sure we don't override a changeTurnRequest when we update the turn timer
        ExtractChangeTurnRequest(out bool changeTurn, out bool changeRound, out int newTeamToPlay);

        if (changeRound)
        {
            World.RoundTime = new FixTimeData(World.RoundTime.ElapsedTime + 1, 1);
        }
        else
        {
            World.RoundTime = new FixTimeData(World.RoundTime.ElapsedTime, 0);
        }

        if (changeTurn)
        {
            World.TurnTime = new FixTimeData(World.TurnTime.ElapsedTime + 1, 1);

            // set new turn
            SetSingleton(new TurnCurrentTeamSingletonComponent { Value = newTeamToPlay });
            SetSingleton(new TurnCountSingletonComponent { Value = GetSingleton<TurnCountSingletonComponent>().Value + 1 });

            switch ((DesignerFriendlyTeam)newTeamToPlay)
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
        else
        {
            World.TurnTime = new FixTimeData(World.TurnTime.ElapsedTime, 0);
        }
    }

    private void ExtractChangeTurnRequest(out bool changeTurn, out bool changeRound, out int newTeamToPlay)
    {
        changeRound = false;
        changeTurn = false;
        newTeamToPlay = 0;

        if (TryGetSingleton(out RequestChangeTurnData requestData))
        {
            DestroySingleton<RequestChangeTurnData>();

            if (CommonReads.GetTurnTeam(Accessor).Value == (int)DesignerFriendlyTeam.Player)
            {
                if (CheckAllPlayersDone())
                {
                    SetSingleton(new PlayersTurn { Value = 1 });
                }
                else
                {
                    OnPlayerCompletedTheirTurn();
                    return;
                }
            }

            changeTurn = true;
            newTeamToPlay = requestData.TeamToPlay;

            // wrap team if necessary
            if (newTeamToPlay >= GetSingleton<TurnTeamCountSingletonComponent>().Value)
            {
                changeRound = true;
                newTeamToPlay = 0;
            }
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
            if (CommonReads.GetTurnTeam(Accessor).Value == (int)DesignerFriendlyTeam.Player)
            {
                OnPlayerCompletedTheirTurn();
                return;
            }

            CommonWrites.RequestNextTurn(Accessor);
        }
        else
        {
            SetSingleton(new TurnTimerSingletonComponent { Value = newTimerValue });
        }
    }

    private void OnPlayerCompletedTheirTurn()
    {
        if (TryGetSingleton(out PlayersTurn playersTurnPlayed))
        {
            if (CheckAllPlayersDone())
            {
                CommonWrites.RequestNextTurn(Accessor);
                return;
            }
            else
            {
                SetSingleton(new PlayersTurn { Value = playersTurnPlayed.Value + 1 });
            }
        }

        SetSingleton(new TurnTimerSingletonComponent { Value = GetSingleton<TurnDurationSingletonComponent>().DurationPlayer });
    }

    private bool CheckAllPlayersDone()
    {
        int playerCount = 0;
        Entities
            .WithAll<PlayerTag>()
            .ForEach((Entity pawnController, ref ControlledEntity pawn) =>
            {
                if (Accessor.Exists(pawn))
                {
                    playerCount++;
                }
            }).WithoutBurst().Run();

        if (TryGetSingleton(out PlayersTurn playersTurnPlayed))
        {
            if (playersTurnPlayed.Value >= playerCount)
            {
                return true;
            }
        }

        return false;
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
