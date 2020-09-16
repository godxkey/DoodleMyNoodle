using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class RefillActionPointsSystem : SimComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<ActionRefillAmount>();
    }

    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetTurnTeam(Accessor);
            int actionPointsToAdd = GetSingleton<ActionRefillAmount>().Value;

            Entities
                .ForEach((Entity pawn, ref ActionPoints actionPoints) =>
                {
                    Entity pawnController = CommonReads.GetPawnController(Accessor, pawn);
                    if (pawnController != Entity.Null)
                    {
                        Team pawnTeam = EntityManager.GetComponentData<Team>(pawnController);

                        if (pawnTeam.Value != currentTeam)
                        {
                            return;
                        }

                        CommonWrites.ModifyStatInt<ActionPoints>(Accessor, pawn, actionPointsToAdd);
                    }
                });
        }
    }
}
