using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class RequestNextTurnIfTeamMembersReadySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        int teamCurrentlyPlaying = CommonReads.GetCurrentTurnTeam(Accessor);
        bool everyoneIsReady = true;
        bool atLeastOneTeamMember = false;
        bool turnIsInvalid = teamCurrentlyPlaying == -1;

        Entities.ForEach((ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            atLeastOneTeamMember = true;

            // if a team member is NOT ready
            if ((turnIsInvalid || (team.Value == teamCurrentlyPlaying)) && !readyForNextTurn.Value)
            {
                // during invalid turn (before game start), we wait only for player approval
                if(turnIsInvalid && (team.Value != (int)TeamAuth.DesignerFriendlyTeam.Baddies))
                {
                    everyoneIsReady = false;
                }
                else if (!turnIsInvalid)
                {
                    everyoneIsReady = false;
                }
            }
        });

        if (everyoneIsReady && atLeastOneTeamMember)
        {
            // Game has started yet ? we're waiting on players to join and create their characters
            if (!Accessor.HasSingleton<GameReadyToStart>())
            {
                // Start Game
                Accessor.SetOrCreateSingleton(new GameReadyToStart());
            }

            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}
