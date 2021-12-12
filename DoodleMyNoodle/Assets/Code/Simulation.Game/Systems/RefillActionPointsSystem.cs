using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class RefillActionPointsSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetTurnTeam(Accessor);

            Entities
                .ForEach((Entity pawn, ref ActionPoints actionPoints, in Controllable controllable) =>
                {
                    Entity pawnController = controllable.CurrentController;
                    if (HasComponent<Team>(pawnController))
                    {
                        Team pawnTeam = GetComponent<Team>(pawnController);

                        if (pawnTeam.Value != currentTeam)
                        {
                            return;
                        }

                        actionPoints = GetComponent<MaximumFix<ActionPoints>>(pawn).Value;
                    }
                })
                .Run();
        }
    }
}
