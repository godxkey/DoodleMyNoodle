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
            CommonWrites.NextTurn(this);
        }
        else
        {
            EntityManager.SetComponentData(turnSystemData, new TurnTimer { Value = newTimerValue });
        }
    }
}

internal static partial class CommonWrites
{
    public static void NextTurn(ComponentSystemBase system)
    {
        Entity turnSystemData = system.GetSingletonEntity<TurnTimer>();
        TurnCurrentTeam turnCurrentTeam = system.EntityManager.GetComponentData<TurnCurrentTeam>(turnSystemData);

        int newCurrentTeam = turnCurrentTeam.Value + 1;
        SetTurn(system, newCurrentTeam);
    }

    public static void SetTurn(ComponentSystemBase system, int team)
    {
        Entity turnSystemData = system.GetSingletonEntity<TurnTimer>();
        TurnDuration turnDuration = system.EntityManager.GetComponentData<TurnDuration>(turnSystemData);
        TurnTeamCount teamAmount = system.EntityManager.GetComponentData<TurnTeamCount>(turnSystemData);

        int newCurrentTeam = team;
        if (newCurrentTeam > teamAmount.Value)
        {
            newCurrentTeam = 0;
        }

        system.EntityManager.SetComponentData(turnSystemData, new TurnCurrentTeam { Value = newCurrentTeam });

        system.EntityManager.SetComponentData(turnSystemData, new TurnTimer { Value = turnDuration.Value });
    }
}
