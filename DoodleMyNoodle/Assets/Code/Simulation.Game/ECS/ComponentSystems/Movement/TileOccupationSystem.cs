using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
public class TileOccupationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Velocity>()
            .ForEach((Entity pawn, ref FixTranslation pawnTranslation, ref PreviousFixTranslation previousPawnTranslation) =>
            {
                int2 tilePosition = CalculateCurrentTilePosition(pawnTranslation.Value);
                Entity currentTile = CommonReads.GetTile(Accessor, tilePosition);

                if (!IsTileAlreadyOccupied(currentTile))
                {
                    Entity newTileEntity = EntityManager.CreateEntity(typeof(FixTranslation), typeof(Occupied));
                    EntityManager.SetComponentData(newTileEntity, new FixTranslation() { Value = fix3(tilePosition, 0) });

#if UNITY_EDITOR
                    EntityManager.SetName(newTileEntity, $"SE_OccupiedTile {tilePosition.x}, {tilePosition.y}");
#endif

                    CommonWrites.AddEntityOnTile(Accessor, newTileEntity, currentTile);
                }

                int2 previousTilePosition = CalculateCurrentTilePosition(previousPawnTranslation.Value);
                Entity previousTile = CommonReads.GetTile(Accessor, previousTilePosition);

                if (currentTile != previousTile)
                {
                    Entity occupiedAddonEntity = CommonReads.GetSingleTileAddonOfType<Occupied>(Accessor, previousTile);
                    CommonWrites.RemoveEntityOnTile(Accessor, occupiedAddonEntity, previousTile);
                    EntityManager.DestroyEntity(occupiedAddonEntity);
                }
            });
        
    }

    private int2 CalculateCurrentTilePosition(fix3 translation)
    {
        return roundToInt(translation).xy;
    }

    private bool IsTileAlreadyOccupied(Entity tile)
    {
        NativeArray<EntityOnTile> entitiesOnTile = CommonReads.GetTileAddons(Accessor, tile);
        foreach (EntityOnTile entity in entitiesOnTile)
        {
            if (Accessor.HasComponent<Occupied>(entity.TileEntity))
            {
                return true;
            }
        }

        return false;
    }
}
