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
        bool atLeastOnePlayerExists = false;

        Entities.ForEach((ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            if (team.Value == (int)TeamAuth.DesignerFriendlyTeam.Player)
            {
                atLeastOnePlayerExists = true;

                if (!readyForNextTurn.Value)
                {
                    everyoneIsReady = false; // if a team member is NOT ready
                }
            }
        });

        // if a team member is NOT ready
        if (teamCurrentlyPlaying == -1 && atLeastOnePlayerExists && !HasSingleton<GameReadyToStart>() && everyoneIsReady)
        {
            this.SetOrCreateSingleton(new GameReadyToStart());

            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}