using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ActionRefillSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);

            Entities
            .ForEach((Entity pawn, ref ActionPoints pawnActionPoints, ref MaximumInt<ActionPoints> pawnMaximumActionPoints) =>
            {
                Entity pawnController = CommonReads.GetPawnController(Accessor, pawn);
                Team pawnTeam = Accessor.GetComponentData<Team>(pawnController);

                if ((pawnTeam.Value != currentTeam) || !HasSingleton<ActionRefillAmount>())
                {
                    return;
                }

                int actionPointsToAdd = GetSingleton<ActionRefillAmount>().Value;

                pawnActionPoints.Value = Mathf.Clamp(pawnActionPoints.Value + actionPointsToAdd, 0, pawnMaximumActionPoints.Value);
            });
        }
    }
}
