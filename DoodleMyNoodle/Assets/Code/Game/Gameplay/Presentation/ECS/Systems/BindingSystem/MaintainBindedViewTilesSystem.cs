using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;

public struct BindedTileTag : ISystemStateComponentData
{
}

public struct BindedTile : ISystemStateComponentData
{
    public TileId Value;

    public static implicit operator TileId(BindedTile val) => val.Value;
    public static implicit operator BindedTile(TileId val) => new BindedTile() { Value = val };
}

[UpdateAfter(typeof(BindedViewEntityCommandBufferSystem))]
public class MaintainBindedViewTilesSystem : ViewSystemBase
{
    List<Vector3Int> _tilePos = new List<Vector3Int>();
    List<TileBase> _tiles = new List<TileBase>();

    protected override void OnUpdate()
    {
        if (!SimAssetBankInstance.Ready || SimTilemapViewInstance.Instance == null)
        {
            return;
        }

        _tilePos.Clear();
        _tiles.Clear();

        DestroyOldTiles(_tilePos, _tiles);
        CreateNewTiles(SimAssetBankInstance.GetLookup(), _tilePos, _tiles);

        if (_tilePos.Count > 0)
        {
            var tilemap = SimTilemapViewInstance.Instance;
            tilemap.SetTiles(_tilePos.ToArray(), _tiles.ToArray());
        }
    }

    private void DestroyOldTiles(List<Vector3Int> outTilePos, List<TileBase> outTiles)
    {
        Entities
            .WithAll<BindedTileTag>()
            .WithNone<BindedSimEntity>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity) =>
            {
                if (EntityManager.TryGetComponentData(entity, out BindedTile tile))
                {
                    outTilePos.Add(new Vector3Int(tile.Value.X, tile.Value.X, 0));
                    outTiles.Add(null);

                    EntityManager.RemoveComponent<BindedTile>(entity);
                }

                EntityManager.RemoveComponent<BindedTileTag>(entity);
            }).Run();
    }

    private void CreateNewTiles(SimAssetBank.Lookup lookup, List<Vector3Int> outTilePos, List<TileBase> outTiles)
    {
        var simEntityPos = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>();
        var simEntityTileIds = SimWorldAccessor.GetComponentDataFromEntity<TileId>();

        Entities
            .WithSharedComponentFilter(new BindedViewType() { Value = ViewTechType.Tile })
            .WithNone<BindedTileTag>()
            .WithoutBurst()
            .WithStructuralChanges()
            .WithReadOnly(simEntityPos)
            .ForEach((Entity viewEntity, in SimAssetId id, in BindedSimEntity simEntity) =>
            {
                TileId? tile = null;

                if (simEntityTileIds.HasComponent(simEntity))
                {
                    tile = simEntityTileIds[simEntity];
                }
                else if (simEntityPos.HasComponent(simEntity))
                {
                    FixTranslation pos = simEntityPos[simEntity];
                    tile = Helpers.GetTileId(pos);
                }

                if(tile != null)
                {
                    SimAsset simAsset = lookup.GetSimAsset(id);

                    if (simAsset.BindedViewTile != null)
                    {
                        TileId t = tile.Value;
                        outTilePos.Add(new Vector3Int(t.X, t.Y, 0));
                        outTiles.Add(simAsset.BindedViewTile);

                        EntityManager.AddComponentData(viewEntity, new BindedTile() { Value = t });
                    }
                }

                EntityManager.AddComponent<BindedTileTag>(viewEntity);
            }).Run();
    }
}