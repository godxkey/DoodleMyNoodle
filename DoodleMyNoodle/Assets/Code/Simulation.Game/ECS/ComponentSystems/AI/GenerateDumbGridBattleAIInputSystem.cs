using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;


/// <summary>
/// This system generate human-like inputs for AIs with the DumbGridBattleAI tag
/// </summary>
public class GenerateDumbGridBattleAIInputSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // the turn that can currently play

        Entities.ForEach((ref NewTurnEventData turnEvent) =>
        {
            //
        });

        Entities
            .WithAll<DumbGridBattleAITag>()
            .ForEach((Entity entity, ref Team team) =>
            {
                if(!CommonReads.CanTeamPlay(Accessor, team))
                {
                    return;
                }

                
            });
    }
}