using Unity.Entities;

public class ChangeTurnSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entity turnSystemData = GetSingletonEntity<TurnTimer>();
        
        TurnTimer turnTimer = EntityManager.GetComponentData<TurnTimer>(turnSystemData);

        fix newTimerValue = turnTimer.Value - Time.DeltaTime;
        if (newTimerValue <= 0)
        {
            CommonWrites.NextTurn(Accessor);
        }
        else
        {
            EntityManager.SetComponentData(turnSystemData, new TurnTimer { Value = newTimerValue });
        }
    }
}

internal static partial class CommonWrites
{
    public static void NextTurn(ISimWorldReadWriteAccessor accessor)
    {
        Entity turnSystemData = accessor.GetSingletonEntity<TurnTimer>();
        TurnCurrentTeam turnCurrentTeam = accessor.GetComponentData<TurnCurrentTeam>(turnSystemData);

        int newCurrentTeam = turnCurrentTeam.Value + 1;
        SetTurn(accessor, newCurrentTeam);
    }

    public static void SetTurn(ISimWorldReadWriteAccessor accessor, int team)
    {
        Entity turnSystemData = accessor.GetSingletonEntity<TurnTimer>();
        TurnDuration turnDuration = accessor.GetComponentData<TurnDuration>(turnSystemData);
        TurnTeamCount teamAmount = accessor.GetComponentData<TurnTeamCount>(turnSystemData);

        int newCurrentTeam = team;
        if (newCurrentTeam > teamAmount.Value)
        {
            newCurrentTeam = 0;
        }

        accessor.SetComponentData(turnSystemData, new TurnCurrentTeam { Value = newCurrentTeam });

        accessor.SetComponentData(turnSystemData, new TurnTimer { Value = turnDuration.Value });
    }
}
