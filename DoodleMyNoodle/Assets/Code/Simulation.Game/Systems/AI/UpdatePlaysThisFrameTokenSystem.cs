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
                    // Cannot play if team cannot play
                    = team.Value == currentTurnTeam

                    // Cannot play if 'ReadyForNextTurn' already done
                    && !readyForNextTurn

                    // Cannot play if action in cooldown
                    && time >= actionCooldown.NoActionUntilTime

                    // Cannot play if pawn doesn't exist
                    && HasComponent<FixTranslation>(pawn)

                    // Cannot play if AI was attempting to move last frame.
                    && (!moveInputLastFrame.WasAttemptingToMove || GetComponent<ActionPoints>(pawn).Value <= 0);

            }).Schedule();
    }
}
