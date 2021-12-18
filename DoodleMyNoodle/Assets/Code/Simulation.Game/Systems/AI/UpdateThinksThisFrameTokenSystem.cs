using CCC.Fix2D;
using Unity.Entities;

public struct ThinksThisFrameCooldownSingleton : IComponentData
{
    public fix NoTokenUntilTime;
}

[UpdateInGroup(typeof(AISystemGroup))]
public class UpdateThinksThisFrameTokenSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<ThinksThisFrameCooldownSingleton>())
        {
            CreateSingleton<ThinksThisFrameCooldownSingleton>();
        }

        int currentTurnTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        bool aisInCooldown = GetSingleton<ThinksThisFrameCooldownSingleton>().NoTokenUntilTime > time;
        bool atLeastOnAIThinks = false;

        Entities
            .ForEach((ref AIThinksThisFrameToken thinksNextUpdate,
                in ReadyForNextTurn readyForNextTurn, in AIActionCooldown actionCooldown, in Team team, in AIMoveInputLastFrame moveInputLastFrame, in ControlledEntity pawn) =>
            {
                thinksNextUpdate.Value
                    // global cooldown
                    = !aisInCooldown && !atLeastOnAIThinks

                    // Cannot play if team cannot play
                    && team.Value == currentTurnTeam

                    // Cannot play if 'ReadyForNextTurn' already done
                    && !readyForNextTurn

                    // Cannot play if action in cooldown
                    && time >= actionCooldown.NoActionUntilTime

                    // Cannot play if pawn doesn't exist
                    && HasComponent<FixTranslation>(pawn)

                    // Cannot play if AI was attempting to move last frame.
                    && (!moveInputLastFrame.WasAttemptingToMove || GetComponent<ActionPoints>(pawn).Value <= 0);

                atLeastOnAIThinks |= thinksNextUpdate.Value;
            }).Run();

        if (atLeastOnAIThinks)
        {
            SetSingleton(new ThinksThisFrameCooldownSingleton() { NoTokenUntilTime = time + SimulationGameConstants.AIThinkGlobalCooldown });
        }
    }
}
