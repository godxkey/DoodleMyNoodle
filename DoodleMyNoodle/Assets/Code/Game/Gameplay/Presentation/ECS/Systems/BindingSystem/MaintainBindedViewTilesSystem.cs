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
public partial class MaintainBindedViewTilesSystem : ViewSystemBase
{
    private struct TilePosPair
    {
        public Vector3Int Pos;
        public TileBase Tile;

        public TilePosPair(Vector3Int pos, TileBase tile)
        {
            this.Pos = pos;
            this.Tile = tile;
        }
    }
    List<TilePosPair> _tileChangeBuffer = new List<TilePosPair>();

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

    private void ApplyTileChanges(List<TilePosPair> tileChanges)
    {
        if (tileChanges.Count == 0)
            return;

        var tilemap = SimTilemapViewInstance.Instance;
        Vector3Int[] pos = new Vector3Int[tileChanges.Count];
        TileBase[] tiles = new TileBase[tileChanges.Count];
        for (int i = 0; i < tileChanges.Count; i++)
        {
            pos[i] = tileChanges[i].Pos;
            tiles[i] = tileChanges[i].Tile;
        }
        tilemap.SetTiles(pos, tiles);
    }

    private void FindOldTiles(List<TilePosPair> outTileChanges)
    {
        Entities
            .WithAll<BindedTileTag>()
            .WithNone<BindedSimEntity>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity) =>
            {
                if (EntityManager.TryGetComponent(entity, out BindedTile tile))
                {
                    outTileChanges.Add(new TilePosPair(new Vector3Int(tile.Value.X, tile.Value.Y, 0), null));

                    EntityManager.RemoveComponent<BindedTile>(entity);
                }

                EntityManager.RemoveComponent<BindedTileTag>(entity);
            }).Run();
    }

    private void FindNewTiles(SimAssetBank.Lookup lookup, List<TilePosPair> outTileChanges)
    {
        var simEntityPos = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>();
        var simEntityTileIds = SimWorldAccessor.GetComponentDataFromEntity<TileId>();

        Entities
            .WithAll<BindedViewType_Tile>()
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
                        outTileChanges.Add(new TilePosPair(new Vector3Int(t.X, t.Y, 0), simAsset.BindedViewTile));

                        EntityManager.AddComponentData(viewEntity, new BindedTile() { Value = t });
                    }
                }

                EntityManager.AddComponent<BindedTileTag>(viewEntity);
            }).Run();
    }
}