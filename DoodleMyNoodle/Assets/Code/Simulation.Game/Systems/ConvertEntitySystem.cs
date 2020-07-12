using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class ConvertEntitySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentPlayingTeam = CommonReads.GetCurrentTurnTeam(Accessor);

            Entities
                .ForEach((Entity entity, ref Converted converted, ref Team team) =>
                {
                    // decrease duration
                    if (team.Value != currentPlayingTeam)
                    {
                        converted.RemainingTurns--;
                    }

                    // switch team if convert is complete
                    if (converted.RemainingTurns <= 0)
                    {
                        team.Value = team.Value == 0 ? 1 : 0;
                        PostUpdateCommands.RemoveComponent<Converted>(entity);
                    }
                });
        }
    }
}
