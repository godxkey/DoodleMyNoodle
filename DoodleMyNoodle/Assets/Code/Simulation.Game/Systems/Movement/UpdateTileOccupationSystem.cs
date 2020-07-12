using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
public class UpdateTileOccupationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities

            .WithAll<Velocity>()
            .ForEach((Entity pawn, ref FixTranslation pawnTranslation, ref PreviousFixTranslation previousPawnTranslation) =>
            {
                int2 tilePosition = CalculateCurrentTilePosition(pawnTranslation.Value);
                Entity currentTile = CommonReads.GetTileEntity(Accessor, tilePosition);

                if (!IsTileAlreadyOccupied(currentTile))
                {
                    Entity newTileEntity = EntityManager.CreateEntity(typeof(FixTranslation), typeof(Occupied));
                    EntityManager.SetComponentData(newTileEntity, new FixTranslation() { Value = fix3(tilePosition, 0) });

#if UNITY_EDITOR
                    EntityManager.SetName(newTileEntity, $"SE_OccupiedTile {tilePosition.x}, {tilePosition.y}");
#endif

                    CommonWrites.AddTileAddon(Accessor, newTileEntity, currentTile);
                }

                int2 previousTilePosition = CalculateCurrentTilePosition(previousPawnTranslation.Value);
                Entity previousTile = CommonReads.GetTileEntity(Accessor, previousTilePosition);

                if (currentTile != previousTile)
                {
                    Entity occupiedAddonEntity = CommonReads.GetFirstTileAddonWithComponent<Occupied>(Accessor, previousTile);
                    CommonWrites.RemoveTileAddon(Accessor, occupiedAddonEntity, previousTile);
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
        DynamicBuffer<TileAddonReference> tileAddons = EntityManager.GetBufferReadOnly<TileAddonReference>(tile);
        foreach (TileAddonReference entity in tileAddons)
        {
            if (EntityManager.HasComponent<Occupied>(entity.Value))
            {
                return true;
            }
        }

        return false;
    }
}
