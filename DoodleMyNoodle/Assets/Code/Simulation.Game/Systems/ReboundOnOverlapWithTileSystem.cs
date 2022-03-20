using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ReboundOnOverlapWithTileSystem : SimGameSystemBase
{
    private List<Entity> _toDestroy = new List<Entity>();

    protected override void OnUpdate()
    {
        Entities
           .WithoutBurst()
           .WithStructuralChanges()
           .ForEach((Entity entity, ref ReboundOnOverlapWithTileState reboundOnOverlapWithTileState, ref PhysicsVelocity physicsVelocity, in ReboundOnOverlapWithTileSetting reboundOnOverlapWithTileSetting, in FixTranslation fixTranslation) =>
           {
               fix2 PossibleNewPosition = fixTranslation.Value;

               Entity tileEntity = CommonReads.GetTileEntity(Accessor, Helpers.GetTile(PossibleNewPosition));
               if (tileEntity != Entity.Null)
               {
                   fix2 tilePos = Helpers.GetTileCenter(EntityManager.GetComponentData<TileId>(tileEntity));
                   if (tilePos != reboundOnOverlapWithTileState.PreviousTile || ((GetElapsedTime(TimeValue.ValueType.Seconds).Value - reboundOnOverlapWithTileState.LastCollisionTime.Value) >= fix.Half))
                   {
                       if (EntityManager.TryGetComponent(tileEntity, out TileFlagComponent tileFlagComponent))
                       {
                           if (!tileFlagComponent.IsEmpty && !tileFlagComponent.IsLadder)
                           {
                               if (reboundOnOverlapWithTileState.ReboundCount < reboundOnOverlapWithTileSetting.ReboundMax)
                               {
                                   reboundOnOverlapWithTileState.PreviousTile = tilePos;
                                   reboundOnOverlapWithTileState.LastCollisionTime = GetElapsedTime(TimeValue.ValueType.Seconds);
                                   reboundOnOverlapWithTileState.ReboundCount++;

                                   fix2 normal = FindNormalFromOverlap(tilePos, PossibleNewPosition);
                                   fix2 reboundVector = physicsVelocity.Linear - (2 * fixMath.dot(physicsVelocity.Linear, normal) * normal);

                                   physicsVelocity.Linear = reboundVector;
                               }
                               else
                               {
                                   _toDestroy.Add(entity);
                               }
                           }
                       }
                   }
               }
           }).Run();

        foreach (var entity in _toDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }

        _toDestroy.Clear();
    }

    private fix2 FindNormalFromOverlap(fix2 tilePos, fix2 entityPos)
    {
        bool isTop = false;
        bool isLeft = false;
        bool isRight = false;
        bool isBottom = false;

        fix2 normalizeEntityPos = (entityPos - tilePos);
        normalizeEntityPos += new fix2() { x = fix.Half, y = fix.Half };

        if (normalizeEntityPos.x >= fix.Zero && normalizeEntityPos.x <= fix.One && normalizeEntityPos.y >= fix.Half && normalizeEntityPos.y <= fix.One)
        {
            isTop = true;
        }

        if (normalizeEntityPos.x >= fix.Zero && normalizeEntityPos.x <= fix.Half && normalizeEntityPos.y >= fix.Zero && normalizeEntityPos.y <= fix.One)
        {
            isLeft = true;
        }

        if (normalizeEntityPos.x >= fix.Half && normalizeEntityPos.x <= fix.One && normalizeEntityPos.y >= fix.Zero && normalizeEntityPos.y <= fix.One)
        {
            isRight = true;
        }

        if (normalizeEntityPos.x >= fix.Zero && normalizeEntityPos.x <= fix.One && normalizeEntityPos.y >= fix.Zero && normalizeEntityPos.y <= fix.Half)
        {
            isBottom = true;
        }

        if (isTop)
        {
            if (isLeft)
            {
                if ((-1 * normalizeEntityPos.x + 1) > normalizeEntityPos.y)
                {
                    return new fix2() { x = -1, y = 0 }; // left
                }
                else
                {
                    return new fix2() { x = 0, y = 1 }; // top
                }
            }
            else if(isRight)
            {
                if (normalizeEntityPos.x > normalizeEntityPos.y)
                {
                    return new fix2() { x = 1, y = 0 }; // right
                }
                else
                {
                    return new fix2() { x = 0, y = 1 }; // top
                }
            }
            else
            {
                return new fix2() { x = 0, y = 1 }; // top
            }
        }

        if (isLeft)
        {
            if (isTop)
            {
                if ((-1 * normalizeEntityPos.x + 1) > normalizeEntityPos.y)
                {
                    return new fix2() { x = -1, y = 0 }; // left
                }
                else
                {
                    return new fix2() { x = 0, y = 1 }; // top
                }
            }
            else if (isBottom)
            {
                if (normalizeEntityPos.x > normalizeEntityPos.y)
                {
                    return new fix2() { x = 0, y = -1 }; // Bottom
                }
                else
                {
                    return new fix2() { x = -1, y = 0 }; // left
                }
            }
            else
            {
                return new fix2() { x = -1, y = 0 }; // left
            }
        }

        if (isRight)
        {
            if (isTop)
            {
                if (normalizeEntityPos.x > normalizeEntityPos.y)
                {
                    return new fix2() { x = 1, y = 0 }; // right
                }
                else
                {
                    return new fix2() { x = 0, y = 1 }; // top
                }
            }
            else if (isBottom)
            {
                if ((-1 * normalizeEntityPos.x + 1) > normalizeEntityPos.y)
                {
                    return new fix2() { x = 0, y = -1 }; // Bottom
                }
                else
                {
                    return new fix2() { x = 1, y = 0 }; // right
                }
            }
            else
            {
                return new fix2() { x = 1, y = 0 }; // right
            }
        }


        if (isBottom)
        {
            if (isLeft)
            {
                if (normalizeEntityPos.x > normalizeEntityPos.y)
                {
                    return new fix2() { x = 0, y = -1 }; // Bottom
                }
                else
                {
                    return new fix2() { x = -1, y = 0 }; // left
                }
            }
            else if (isRight)
            {
                if ((-1 * normalizeEntityPos.x + 1) > normalizeEntityPos.y)
                {
                    return new fix2() { x = 0, y = -1 }; // Bottom
                }
                else
                {
                    return new fix2() { x = 1, y = 0 }; // right
                }
            }
            else
            {
                return new fix2() { x = 0, y = -1 }; // Bottom
            }
        }

        return new fix2();
    }
}