﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;
using UnityEngine.Serialization;
using System.Linq;

[DisallowMultipleComponent]
public class LevelGridAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [FormerlySerializedAs("LevelGridSetting")]
    [SerializeField] private LevelGridSettings _globalGridSettings;
    [SerializeField] private Grid _grid;
    [SerializeField] private SimAsset _prefabSimAsset;

    public LevelGridSettings GlobalGridSettings { get => _globalGridSettings; set => _globalGridSettings = value; }
    public Grid Grid { get => _grid; set => _grid = value; }
    public SimAsset PrefabSimAsset { get => _prefabSimAsset; set => _prefabSimAsset = value; }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Tilemap[] tileMaps = _grid.GetComponentsInChildren<Tilemap>();

        // compress bounds (recalculates the tileMap.cellBounds)
        foreach (Tilemap tm in tileMaps)
            tm.CompressBounds();

        // find tile map size
        Tilemap[] gameplayTilemaps = tileMaps.Where(t => t.GetComponent<TilemapRenderer>().sortingLayerName == GameConstants.LAYER_GRID_SIMULATION).ToArray();
        FindGridMinMax(gameplayTilemaps, out int2 minCoord, out int2 maxCoord);

        dstManager.AddComponentData(entity, new GridInfo()
        {
            TileMin = minCoord,
            TileMax = maxCoord,
        });
        dstManager.AddComponentData(entity, new DefaultTilesInfo()
        {
            DefaultEmptyTile = SimAssetId.Invalid,
            DefaultLadderTile = _globalGridSettings.DefaultLadderTile.GetSimAssetId(),
            DefaultTerrainTile = _globalGridSettings.DefaultTerrainTile.GetSimAssetId(),
        });
        dstManager.AddBuffer<StartingTileActorElement>(entity);
        dstManager.AddBuffer<StartingTileElement>(entity);

        // Lets get all tilemaps and spawn whatever was drawn on them
        DynamicBuffer<StartingTileActorElement> startingEntities = dstManager.GetBuffer<StartingTileActorElement>(entity);
        DynamicBuffer<StartingTileElement> startingTiles = dstManager.GetBuffer<StartingTileElement>(entity);

        foreach (Tilemap tilemap in gameplayTilemaps)
        {
            // VE is already done in grid generator
            for (int y = minCoord.y; y <= maxCoord.y; y++)
            {
                for (int x = minCoord.x; x <= maxCoord.x; x++)
                {
                    Vector3 worldPos = new Vector3(x, y, 0);
                    Vector3Int gridCell = _grid.WorldToCell(worldPos);

                    GameObject simEntityPrefab = _globalGridSettings.GetSimEntityPrefabFromTile(tilemap.GetTile(gridCell));

                    if (simEntityPrefab != null)
                    {
                        int2 tilePos = int2((int)worldPos.x, (int)worldPos.y);
                        if (simEntityPrefab.TryGetComponent(out TileAuth tileActorAuth))
                        {
                            var simAssetId = simEntityPrefab.GetComponent<SimAsset>();
                            startingTiles.Add(new StartingTileElement()
                            {
                                AssetId = simAssetId != null ? simAssetId.GetSimAssetId() : SimAssetId.Invalid,
                                Position = tilePos,
                                TileFlags = tileActorAuth.GetTileFlags()
                            });
                        }
                        else
                        {
                            Entity tileActorPrefab = conversionSystem.GetPrimaryEntity(simEntityPrefab);
                            startingEntities.Add(new StartingTileActorElement() { Prefab = tileActorPrefab, Position = tilePos });
                        }
                    }
                }
            }
        }

        dstManager.AddComponentData(entity, _prefabSimAsset.GetSimAssetId());
    }

    private static void FindGridMinMax(Tilemap[] tileMaps, out int2 minCoord, out int2 maxCoord)
    {
        if (tileMaps.Length > 0)
        {
            minCoord = int2(int.MaxValue, int.MaxValue);
            maxCoord = int2(int.MinValue, int.MinValue);

            foreach (Tilemap tm in tileMaps)
            {
                BoundsInt cellBounds = tm.cellBounds;

                minCoord = min(int2(cellBounds.xMin, cellBounds.yMin), minCoord);
                maxCoord = max(int2(cellBounds.xMax - 1, cellBounds.yMax - 1 + 1 /* include walking tile */), maxCoord);
            }
        }
        else
        {
            minCoord = int2(0, 0);
            maxCoord = int2(0, 0);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (EntitySpriteBinding binding in _globalGridSettings.SimEntitySpriteBindings)
        {
            // skip tile actors meant for tiles
            if (binding.EntityPrefab.TryGetComponent(out TileAuth tileActorAuth) &&
                tileActorAuth.ShouldBeConvertedToTile())
            {
                continue;
            }

            referencedPrefabs.Add(binding.EntityPrefab);
        }
    }

    void OnValidate()
    {
        if (_grid == null)
        {
            Log.Warning($"({gameObject.name}) Level Grid Auth needs a grid", gameObject);
            return;
        }

        if (_grid.CellToWorld(Vector3Int.zero) != Vector3.zero)
        {
            Log.Error($"({gameObject.name}) Grid position should be (0, 0, 0). Fixing!", gameObject);
            _grid.transform.position = Vector3.zero;
        }
    }
}
