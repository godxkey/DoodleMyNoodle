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

            Entities.ForEach((Entity entity, ref ActionPoints moveEnergy, in Controllable controllable) =>
            {
                if (HasComponent<Team>(controllable.CurrentController))
                {
                    if (GetComponent<Team>(controllable.CurrentController).Value != currentTeam)
                    {
                        moveEnergy.Value = 0;
                    }
                }
            }).Run();
        }
    }
}
