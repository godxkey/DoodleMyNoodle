using System;
using System.Collections.Generic;
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
            .ForEach((Entity pawn, ref FixTranslation pawnTranslation) =>
            {
                int2 tilePosition = CalculateCurrentTilePosition(pawn);

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
            });
        
    }

    private int2 CalculateCurrentTilePosition(Entity pawn)
    {
        fix3 pawnPosition = Accessor.GetComponentData<FixTranslation>(pawn).Value;

        return roundToInt(pawnPosition).xy;
    }

    private bool IsTileAlreadyOccupied(Entity tile)
    {
        List<Entity> entitiesOnTile = CommonReads.GetTileAddons(Accessor, tile);
        foreach (Entity entity in entitiesOnTile)
        {
            if (Accessor.HasComponent<Occupied>(entity))
            {
                return true;
            }
        }

        return false;
    }
}
