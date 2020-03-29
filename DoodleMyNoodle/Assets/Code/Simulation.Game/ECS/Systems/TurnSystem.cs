using Unity.Entities;

public class TurnSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entity turnSystemData = GetSingletonEntity<TurnTimer>();
        TurnDuration turnDuration = EntityManager.GetComponentData<TurnDuration>(turnSystemData);
        TurnCurrentTeam turnCurrentTeam = EntityManager.GetComponentData<TurnCurrentTeam>(turnSystemData);
        MaximumInt<TurnCurrentTeam> teamAmount = EntityManager.GetComponentData<MaximumInt<TurnCurrentTeam>>(turnSystemData);
        TurnTimer turnTimer = EntityManager.GetComponentData<TurnTimer>(turnSystemData);

        turnTimer.Value -= Time.DeltaTime;

        if (turnTimer.Value <= 0)
        {
            turnCurrentTeam.Value++;
            if(turnCurrentTeam.Value > teamAmount.Value) 
            {
                turnCurrentTeam.Value = 0;
            }

            turnTimer.Value = turnDuration.Value;
        }
    }
}
