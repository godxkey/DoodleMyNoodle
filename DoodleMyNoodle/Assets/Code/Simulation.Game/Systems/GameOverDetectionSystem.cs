using Unity.Entities;

public struct CanTriggerGameOverTag : IComponentData { }

public class GameOverDetectionSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // We cannot trigger the 'Game Over' until the two teams have at least had 1 member.
        // Otherwise, entering a game with no enemy triggers GameOver instantly.
        if (!HasSingleton<CanTriggerGameOverTag>())
        {
            if (CommonReads.GetEntitiesFromTeam(Accessor, (int)TeamAuth.DesignerFriendlyTeam.Baddies).Length > 0 &&
                CommonReads.GetEntitiesFromTeam(Accessor, (int)TeamAuth.DesignerFriendlyTeam.Player).Length > 0)
            {
                this.CreateSingleton<CanTriggerGameOverTag>();
            }
        }

        // Upon new turn, check if a team is empty
        if (HasSingleton<NewTurnEventData>() && HasSingleton<CanTriggerGameOverTag>())
        {
            int playerAlive = 0;
            int aiAlive = 0;

            Entities.ForEach((ref Team pawnControllerTeam, ref ControlledEntity pawn) =>
            {
                // if the team member controls a pawn with Health
                if(EntityManager.Exists(pawn.Value) && EntityManager.HasComponent<Health>(pawn.Value))
                {
                    if (pawnControllerTeam.Value == (int)TeamAuth.DesignerFriendlyTeam.Baddies)
                    {
                        aiAlive++;
                    }
                    else if (pawnControllerTeam.Value == (int)TeamAuth.DesignerFriendlyTeam.Player)
                    {
                        playerAlive++;
                    }
                }
            });

            if (aiAlive <= 0)
            {
                // Player wins !
                if (!HasSingleton<WinningTeam>())
                {
                    this.CreateSingleton(new WinningTeam { Value = (int)TeamAuth.DesignerFriendlyTeam.Player });
                }
            }
            else if (playerAlive <= 0)
            {
                // AI wins !
                if (!HasSingleton<WinningTeam>())
                {
                    this.CreateSingleton(new WinningTeam { Value = (int)TeamAuth.DesignerFriendlyTeam.Baddies });
                }
            }
        }
    }
}
