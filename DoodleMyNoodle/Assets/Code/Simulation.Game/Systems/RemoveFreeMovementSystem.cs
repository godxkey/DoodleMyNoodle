using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class RemoveFreeMovementSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetTurnTeam(Accessor);

            Entities.ForEach((Entity entity, ref CanMoveFreely canMoveFreely, ref MoveInput moveInput) =>
            {
                Entity pawnController = CommonReads.GetPawnController(Accessor, entity);
                if (pawnController != Entity.Null)
                {
                    if (EntityManager.GetComponentData<Team>(pawnController).Value != currentTeam)
                    {
                        canMoveFreely = new CanMoveFreely() { CanMove = false };
                        moveInput = new MoveInput() { Value = new fix2(0, 0) };
                    }
                }
            });
        }

        Entities.ForEach((Entity entity, ref CanMoveFreely canMoveFreely, ref MoveInput moveInput, ref MoveEnergy moveEnergy) =>
        {
            if (moveEnergy.Value <= 0)
            {
                canMoveFreely = new CanMoveFreely() { CanMove = false };
                moveInput = new MoveInput() { Value = new fix2(0, 0) };
            }
        });
    }
}
