using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LevelGridAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [FormerlySerializedAs("LevelGridSetting")]
    [SerializeField] private LevelGridSettings _globalGridSettings;
    [SerializeField] private Grid _grid;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Tilemap[] tileMaps = _grid.GetComponentsInChildren<Tilemap>();

        // compress bounds (recalculates the tileMap.cellBounds)
        foreach (Tilemap tm in tileMaps)
            tm.CompressBounds();

        // find tile map size
        FindGridMinMax(_grid, tileMaps, out int2 minCoord, out int2 maxCoord);

        dstManager.AddComponentData(entity, new GridInfo()
        {
            TileMin = minCoord,
            TileMax = maxCoord,
        });

        // Lets get all tilemaps and spawn whatever was drawn on them
        DynamicBuffer<StartingTileActors> startingEntities = dstManager.AddBuffer<StartingTileActors>(entity);

        foreach (Tilemap tileMap in tileMaps)
        {
            for (int y = minCoord.y; y <= maxCoord.y; y++)
            {
                for (int x = minCoord.x; x <= maxCoord.x; x++)
                {
                    Vector3 worldPos = new Vector3(x, y, 0);
                    Vector3Int gridCell = _grid.WorldToCell(worldPos);

                    GameObject simEntityPrefab = _globalGridSettings.GetSimEntityPrefabFromSprite(tileMap.GetSprite(gridCell));

                    if (simEntityPrefab != null)
                    {
                        Entity tileActorPrefab = conversionSystem.GetPrimaryEntity(simEntityPrefab);
                        int2 tilePos = int2((int)worldPos.x, (int)worldPos.y);

                        startingEntities.Add(new StartingTileActors() { Prefab = tileActorPrefab, Position = tilePos });
                    }
                }
            }
        }
    }

    private static void FindGridMinMax(Grid grid, Tilemap[] tileMaps, out int2 minCoord, out int2 maxCoord)
    {
        if (tileMaps.Length > 0)
        {
            minCoord = int2(int.MaxValue, int.MaxValue);
            maxCoord = int2(int.MinValue, int.MinValue);

            foreach (Tilemap tm in tileMaps)
            {
                BoundsInt cellBounds = tm.cellBounds;

                minCoord = min(int2(cellBounds.xMin, cellBounds.yMin), minCoord);
                maxCoord = max(int2(cellBounds.xMax - 1, cellBounds.yMax - 1), maxCoord);
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
            referencedPrefabs.Add(binding.EntityPrefab);
        }
    }

    void OnValidate()
    {
        if(_grid == null)
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
