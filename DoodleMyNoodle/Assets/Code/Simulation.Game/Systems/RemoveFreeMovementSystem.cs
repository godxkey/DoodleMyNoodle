using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class RemoveFreeMovementSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetTurnTeam(Accessor);

            Entities.ForEach((Entity entity, ref MoveEnergy moveEnergy) =>
            {
                Entity pawnController = CommonReads.GetPawnController(Accessor, entity);
                if (pawnController != Entity.Null)
                {
                    if (EntityManager.GetComponentData<Team>(pawnController).Value != currentTeam)
                    {
                        moveEnergy.Value = 0;
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}
