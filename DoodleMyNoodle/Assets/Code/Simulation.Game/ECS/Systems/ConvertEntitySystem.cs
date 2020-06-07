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
        ApplyConvertedState();

        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);

            Entities
            .ForEach((Entity entity, ref Converted converted) =>
            {
                Team entityTeam = Accessor.GetComponentData<Team>(entity);

                if ((entityTeam.Value == currentTeam))
                {
                    return;
                }

                if (converted.Duration <= 1)
                {
                    Accessor.SetComponentData(entity, new Team() { Value = (int)TeamAuth.DesignerFriendlyTeam.Baddies });
                    Accessor.RemoveComponent<Converted>(entity);
                }
                else
                {
                    Accessor.SetComponentData(entity, new Converted() { Duration = converted.Duration - 1 });
                }
            });
        }
    }

    private void ApplyConvertedState()
    {
        Entities
            .ForEach((Entity entity, ref NeedToBeConverted needsConvert) =>
            {
                Accessor.SetComponentData(entity, new Team() { Value = (int)TeamAuth.DesignerFriendlyTeam.Player });

                PostUpdateCommands.RemoveComponent<NeedToBeConverted>(entity);
            });
    }
}
