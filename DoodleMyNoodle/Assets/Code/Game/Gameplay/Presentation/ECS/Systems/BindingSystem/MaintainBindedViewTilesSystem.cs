using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using CCC.Fix2D;
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
    List<(Vector3Int pos, TileBase tile)> _tileChangeBuffer = new List<(Vector3Int pos, TileBase tile)>();

    protected override void OnUpdate()
    {
        if (!SimAssetBankInstance.Ready || SimTilemapViewInstance.Instance == null)
        {
            return;
        }

        // scan over new and old bindings

        _tileChangeBuffer.Clear();
        FindOldTiles(outTileChanges: _tileChangeBuffer);
        FindNewTiles(SimAssetBankInstance.GetLookup(), outTileChanges: _tileChangeBuffer);
        ApplyTileChanges(_tileChangeBuffer);
    }

    private void ApplyTileChanges(List<(Vector3Int pos, TileBase tile)> tileChanges)
    {
        if (tileChanges.Count == 0)
            return;

        var tilemap = SimTilemapViewInstance.Instance;
        Vector3Int[] pos = new Vector3Int[tileChanges.Count];
        TileBase[] tiles = new TileBase[tileChanges.Count];
        for (int i = 0; i < tileChanges.Count; i++)
        {
            pos[i] = tileChanges[i].pos;
            tiles[i] = tileChanges[i].tile;
        }
        tilemap.SetTiles(pos, tiles);
    }

    private void FindOldTiles(List<(Vector3Int, TileBase)> outTileChanges)
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
                    outTileChanges.Add((new Vector3Int(tile.Value.X, tile.Value.Y, 0), null));

                    EntityManager.RemoveComponent<BindedTile>(entity);
                }

                EntityManager.RemoveComponent<BindedTileTag>(entity);
            }).Run();
    }

    private void FindNewTiles(SimAssetBank.Lookup lookup, List<(Vector3Int, TileBase)> outTileChanges)
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

                if (tile != null)
                {
                    SimAsset simAsset = lookup.GetSimAsset(id);

                    if (simAsset.BindedViewTile != null)
                    {
                        TileId t = tile.Value;
                        outTileChanges.Add((new Vector3Int(t.X, t.Y, 0), simAsset.BindedViewTile));

                        EntityManager.AddComponentData(viewEntity, new BindedTile() { Value = t });
                    }
                }

                EntityManager.AddComponent<BindedTileTag>(viewEntity);
            }).Run();
    }
}