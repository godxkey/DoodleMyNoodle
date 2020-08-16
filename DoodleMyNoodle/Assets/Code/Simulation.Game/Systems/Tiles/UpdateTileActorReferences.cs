using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct TileActorSystemState : ISystemStateComponentData
{
    public int2 Tile;
}

public class UpdateTileActorReferences : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // Add 'TileActorSystemState' to all new entities
        Entities
            .WithAll<TileActorTag>()
            .WithNone<TileActorSystemState>()
            .ForEach((Entity entity, ref FixTranslation translation) =>
            {
                // Add 'TileActorSystemState'
                int2 tile = Helpers.GetTile(translation);
                
                EntityManager.AddComponentData(entity, new TileActorSystemState() { Tile = tile });

                AddToTile(entity, tile);
            });

        // Update tile for all tile actors
        Entities
            .WithAll<TileActorTag>()
            .WithNone<StaticTag>()
            .ForEach((Entity entity, ref FixTranslation translation, ref TileActorSystemState systemState) =>
            {
                // Add 'TileActorSystemState'
                int2 newTile = Helpers.GetTile(translation);

                if (!newTile.Equals(systemState.Tile))
                {
                    RemoveFromTile(entity, systemState.Tile);
                    AddToTile(entity, newTile);

                    systemState.Tile = newTile;
                }
            });

        // Remove destroyed actors from tiles
        Entities
            .WithNone<TileActorTag>()
            .ForEach((Entity entity, ref TileActorSystemState systemState) =>
            {
                RemoveFromTile(entity, systemState.Tile);
                EntityManager.RemoveComponent<TileActorSystemState>(entity);
            });
    }

    private void AddToTile(Entity entity, int2 tile)
    {
        Entity tileEntity = CommonReads.GetTileEntity(Accessor, tile);
        if (tileEntity != Entity.Null)
        {
            EntityManager.GetBuffer<TileActorReference>(tileEntity).Add(entity);
        }
    }

    private void RemoveFromTile(Entity entity, int2 tile)
    {
        Entity tileEntity = CommonReads.GetTileEntity(Accessor, tile);
        if (tileEntity != Entity.Null)
        {
            EntityManager.GetBuffer<TileActorReference>(tileEntity).RemoveFirst(entity);
        }
    }
}