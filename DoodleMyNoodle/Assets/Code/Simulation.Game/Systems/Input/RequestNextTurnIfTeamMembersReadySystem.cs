using Unity.Entities;

[UpdateInGroup(typeof(InputSystemGroup))]
public class RequestNextTurnIfTeamMembersReadySystem : SimComponentSystem
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

        Entities.ForEach((ref Active active, ref ControlledEntity pawn, ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            // if a team member is NOT ready
            if (active && EntityManager.Exists(pawn) && team == teamCurrentlyPlaying && !readyForNextTurn.Value)
            {
                everyoneIsReady = false;
            }
        });

        if (everyoneIsReady && teamCurrentlyPlaying != -1)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}
