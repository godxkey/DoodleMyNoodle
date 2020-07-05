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
        bool atLeastOnePlayerExist = false;

        Entities.ForEach((ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            if (team.Value == (int)TeamAuth.DesignerFriendlyTeam.Player)
            {
                atLeastOnePlayerExist = true;

                if (!readyForNextTurn.Value)
                {
                    everyoneIsReady = false; // if a team member is NOT ready
                }
            }
        });

        // if a team member is NOT ready
        if (teamCurrentlyPlaying == -1 && atLeastOnePlayerExist && !Accessor.HasSingleton<GameReadyToStart>() && everyoneIsReady)
        {
            Accessor.SetOrCreateSingleton(new GameReadyToStart());

            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}