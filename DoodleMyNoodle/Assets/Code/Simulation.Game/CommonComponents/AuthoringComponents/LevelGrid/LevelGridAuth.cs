using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LevelGridAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public LevelGridSettings LevelGridSetting;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Lets get all tilemaps and spawn whatever was drawn on them

        int gridSize = LevelGridSetting.GridSize;
        dstManager.AddComponentData(entity, new GridInfo() { GridRect = new intRect(-gridSize / 2, -gridSize / 2, gridSize, gridSize) });
        DynamicBuffer<StartingTileAddonData> TileAddons = dstManager.AddBuffer<StartingTileAddonData>(entity);

        Grid[] grids = gameObject.GetComponentsInChildren<Grid>();
        foreach (Grid grid in grids)
        {
            Tilemap[] tileMaps = grid.GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tileMap in tileMaps)
            {
                // middle row and same amount on each side
                int halfGridSize = Mathf.CeilToInt(LevelGridSetting.GridSize / 2f);

                for (int l = -halfGridSize; l <= halfGridSize; l++)
                {
                    for (int h = -halfGridSize; h <= halfGridSize; h++)
                    {
                        Vector3 worldCoordinate = new Vector3(l, h, 0);
                        Vector3Int gridCoordinate = grid.WorldToCell(worldCoordinate);

                        GameObject tileAddonSimulationPrefab = LevelGridSetting.GetSimEntityPrefabFromSprite(tileMap.GetSprite(gridCoordinate));

                        if(tileAddonSimulationPrefab != null)
                        {
                            Entity tileAddonEntity = conversionSystem.GetPrimaryEntity(tileAddonSimulationPrefab);
                            fix2 tileAddonPosition = new fix2(x: (fix)worldCoordinate.x, y: (fix)worldCoordinate.y);
                            StartingTileAddonData newTileAddonData = new StartingTileAddonData() { Prefab = tileAddonEntity, Position = tileAddonPosition };
                            TileAddons.Add(newTileAddonData);
                        }
                    }
                }
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (TileAddonsDefinition TileAddonsDef in LevelGridSetting.AddonsDefinition)
        {
            referencedPrefabs.Add(TileAddonsDef.AddonSimulationPrefab);
        }
    }
}
