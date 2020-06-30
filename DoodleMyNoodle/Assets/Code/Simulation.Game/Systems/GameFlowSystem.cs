using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class GameFlowSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        int teamCurrentlyPlaying = CommonReads.GetCurrentTurnTeam(Accessor);

        bool everyoneIsReady = true;

        Entities.ForEach((ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            // if a team member is NOT ready
            if (team.Value == teamCurrentlyPlaying && !readyForNextTurn.Value)
            {
                everyoneIsReady = false;
            }
        });

        // if a team member is NOT ready
        if (teamCurrentlyPlaying == -1 && !Accessor.HasSingleton<GameReadyToStart>() && everyoneIsReady)
        {
            Accessor.SetOrCreateSingleton(new GameReadyToStart());

            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}