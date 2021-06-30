using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct CanTriggerGameOverTag : IComponentData { }

public struct GameStartedTag : IComponentData { }

public class GameFlowSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        StartGameIfNeeded();
        EndGameIfNeeded();
    }

    private void StartGameIfNeeded()
    {
        if (HasSingleton<GameStartedTag>())
            return;

        int teamCurrentlyPlaying = CommonReads.GetTurnTeam(Accessor);

        // if a team member is NOT ready
        if (teamCurrentlyPlaying == -1)
        {
            bool everyoneIsReady = true;
            bool atLeastOnePlayerExists = false;

            // check if every player is ready
            Entities
                .WithNone<AITag>() // ignore AI
                .ForEach((in Team team, in Active active, in ReadyForNextTurn readyForNextTurn, in ControlledEntity pawn) =>
            {
                if (active && team == (int)DesignerFriendlyTeam.Player)
                {
                    atLeastOnePlayerExists = true;

                    if (!readyForNextTurn && HasComponent<Controllable>(pawn))
                    {
                        everyoneIsReady = false; // if a team member is NOT ready
                    }
                }
            }).Run();

            if (atLeastOnePlayerExists && everyoneIsReady)
            {
                CreateSingleton<GameStartedTag>();
                CommonWrites.RequestNextTurn(Accessor);
            }
        }
    }

    private void EndGameIfNeeded()
    {
        // We cannot trigger the 'Game Over' until the two teams have at least had 1 member.
        // Otherwise, entering a game with no enemy triggers GameOver instantly.
        if (!HasSingleton<CanTriggerGameOverTag>())
        {
            if (CommonReads.GetEntitiesFromTeam(Accessor, (int)DesignerFriendlyTeam.Baddies).Length > 0 &&
                CommonReads.GetEntitiesFromTeam(Accessor, (int)DesignerFriendlyTeam.Player).Length > 0)
            {
                CreateSingleton<CanTriggerGameOverTag>();
            }
        }

        // Upon new turn, check if a team is empty
        if (HasSingleton<NewTurnEventData>() && HasSingleton<CanTriggerGameOverTag>())
        {
            int playerAlive = 0;
            int aiAlive = 0;

            Entities.ForEach((in Team pawnControllerTeam, in ControlledEntity pawn) =>
            {
                // if the team member controls a pawn with Health
                if (HasComponent<Health>(pawn.Value))
                {
                    if (pawnControllerTeam.Value == (int)DesignerFriendlyTeam.Baddies)
                    {
                        aiAlive++;
                    }
                    else if (pawnControllerTeam.Value == (int)DesignerFriendlyTeam.Player)
                    {
                        playerAlive++;
                    }
                }
            }).Run();

            if (aiAlive <= 0)
            {
                // Player wins !
                if (!HasSingleton<WinningTeam>())
                {
                    CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Player });
                }
            }
            else if (playerAlive <= 0)
            {
                // AI wins !
                if (!HasSingleton<WinningTeam>())
                {
                    CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Baddies });
                }
            }
        }
    }
}