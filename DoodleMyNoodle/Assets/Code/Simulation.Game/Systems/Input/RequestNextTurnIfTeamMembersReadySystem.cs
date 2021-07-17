using Unity.Entities;

[UpdateInGroup(typeof(InputSystemGroup))]
public class RequestNextTurnIfTeamMembersReadySystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<TurnCurrentTeamSingletonComponent>();
    }

    protected override void OnUpdate()
    {
        int teamCurrentlyPlaying = GetSingleton<TurnCurrentTeamSingletonComponent>().Value;
        bool everyoneIsReady = true;

        Entities.ForEach((in Active active, in ControlledEntity pawn, in Team team, in ReadyForNextTurn readyForNextTurn) =>
        {
            // if a team member is NOT ready
            if (active && HasComponent<Controllable>(pawn) && team == teamCurrentlyPlaying && !readyForNextTurn.Value)
            {
                everyoneIsReady = false;
            }
        }).Run();

        if (everyoneIsReady && teamCurrentlyPlaying != -1)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}
