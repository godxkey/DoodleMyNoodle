using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LevelGridAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int Size = 100;
    public LevelGridSettings LevelGridSetting;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Lets get all tilemaps and spawn whatever was drawn on them

        int min = -Mathf.FloorToInt((Size - 1) / 2f);
        int max = Mathf.FloorToInt(Size / 2f);

        dstManager.AddComponentData(entity, new GridInfo()
        {
            TileMin = min,
            TileMax = max,
        });

        DynamicBuffer<StartingTileActors> TileAddons = dstManager.AddBuffer<StartingTileActors>(entity);

        Grid[] grids = gameObject.GetComponentsInChildren<Grid>();
        foreach (Grid grid in grids)
        {
            Tilemap[] tileMaps = grid.GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tileMap in tileMaps)
            {
                // middle row and same amount on each side
                int halfGridSize = Mathf.CeilToInt(Size / 2f);

                for (int l = -halfGridSize; l <= halfGridSize; l++)
                {
                    for (int h = -halfGridSize; h <= halfGridSize; h++)
                    {
                        Vector3 worldCoordinate = new Vector3(l, h, 0);
                        Vector3Int gridCoordinate = grid.WorldToCell(worldCoordinate);

                        GameObject tileAddonSimulationPrefab = LevelGridSetting.GetSimEntityPrefabFromSprite(tileMap.GetSprite(gridCoordinate));

                        if (tileAddonSimulationPrefab != null)
                        {
                            Entity tileActorPrefab = conversionSystem.GetPrimaryEntity(tileAddonSimulationPrefab);
                            int2 tilePos = new int2(Mathf.RoundToInt(worldCoordinate.x), Mathf.RoundToInt(worldCoordinate.y));

                            TileAddons.Add(new StartingTileActors() { Prefab = tileActorPrefab, Position = tilePos });
                        }
                    }
                }
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (EntitySpriteBinding binding in LevelGridSetting.SimEntitySpriteBindings)
        {
            referencedPrefabs.Add(binding.EntityPrefab);
        }
    }
}
