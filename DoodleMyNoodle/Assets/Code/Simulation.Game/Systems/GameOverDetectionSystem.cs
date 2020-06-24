using Unity.Entities;
using UnityEngine;

public class GameOverDetectionSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int playerAlive = 0;
            int aiAlive = 0;

            Entities
                .ForEach((Entity pawn, ref ControllableTag tag, ref Health health) =>
                {
                    Entity pawnController = CommonReads.GetPawnController(Accessor, pawn);

                    if (Accessor.HasComponent<Team>(pawnController))
                    {
                        Team pawnTeam = Accessor.GetComponentData<Team>(pawnController);

                        if (pawnTeam.Value == (int)TeamAuth.DesignerFriendlyTeam.Baddies)
                        {
                            aiAlive++;
                        }
                        else if (pawnTeam.Value == (int)TeamAuth.DesignerFriendlyTeam.Player)
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
                    Accessor.SetOrCreateSingleton(new WinningTeam { Value = (int)TeamAuth.DesignerFriendlyTeam.Player });
                }
            }
            else if (playerAlive <= 0)
            {
                // AI wins !
                if (!HasSingleton<WinningTeam>())
                {
                    Accessor.SetOrCreateSingleton(new WinningTeam { Value = (int)TeamAuth.DesignerFriendlyTeam.Baddies });
                }
            }
        }
    }
}
