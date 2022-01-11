using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class ConvertEntitySystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            var currentTurnData = CommonReads.GetCurrentTurnData(Accessor);

            Entities
                .ForEach((Entity entity, ref Converted converted, ref Team team) =>
                {
                    // switch team if convert is complete
                    if (converted.RemainingTurns <= 0)
                    {
                        team.Value = team.Value == 0 ? 1 : 0;
                        EntityManager.RemoveComponent<Converted>(entity);
                    }

                    // decrease duration
                    if (Helpers.CanControllerPlay(entity, currentTurnData))
                    {
                        converted.RemainingTurns--;
                    }
                })
                .WithStructuralChanges()
                .WithoutBurst()
                .Run();
        }
    }
}
