using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;

public class RequestNextTurnIfTeamMembersReadySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        int teamCurrentlyPlaying = CommonReads.GetCurrentTurnTeam(Accessor);
        bool everyoneIsReady = true;
        bool atLeastOneReady = false;

        foreach (Unity.Entities.Entity pawnController in CommonReads.GetEntitiesFromTeam(Accessor, teamCurrentlyPlaying))
        {
            if (Accessor.TryGetComponentData(pawnController, out ReadyForNextTurn IsReady))
            {
                if (!IsReady.Value)
                {
                    // he toggle it off so he's not ready
                    everyoneIsReady = false;
                }
                else
                {
                    atLeastOneReady = true;
                }
            }
            else
            {
                // No Component, he's not ready
                everyoneIsReady = false;
            }
        }

        if (everyoneIsReady && atLeastOneReady)
        {
            // Clear all Ready For Next Turn
            foreach (Unity.Entities.Entity pawnController in CommonReads.GetEntitiesFromTeam(Accessor, teamCurrentlyPlaying))
            {
                Accessor.RemoveComponent<ReadyForNextTurn>(pawnController);
            }

            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}
