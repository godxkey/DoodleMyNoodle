using CCC.Fix2D;
using Unity.Entities;

[UpdateInGroup(typeof(AISystemGroup))]
public class UpdatePlaysThisFrameTokenSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        int currentTurnTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Entities
            .ForEach((ref AIPlaysThisFrameToken playsThisFrame,
                in ReadyForNextTurn readyForNextTurn, in AIActionCooldown actionCooldown, in Team team, in AIMoveInputLastFrame moveInputLastFrame, in ControlledEntity pawn) =>
            {

                playsThisFrame.Value
                    // Can the corresponding team play ?
                    = team.Value == currentTurnTeam

                    // Have we already said 'ready for next turn' ?
                    && !readyForNextTurn

                    // Is the ai in cooldown?
                    && time >= actionCooldown.NoActionUntilTime

                    // Was the ai attempting to move last frame?
                    && !moveInputLastFrame.WasAttemptingToMove

                    // Pawn exists ?
                    && HasComponent<FixTranslation>(pawn);
            }).Schedule();
    }
}
