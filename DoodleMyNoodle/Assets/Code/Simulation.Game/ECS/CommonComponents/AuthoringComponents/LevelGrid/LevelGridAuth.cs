using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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

        dstManager.AddComponentData(entity, new GridInfoContainer() { GridSize = LevelGridSetting.GridSize });
        DynamicBuffer<StartingTileAddonData> TileAddons = dstManager.AddBuffer<StartingTileAddonData>(entity);

        Grid[] grids = gameObject.GetComponentsInChildren<Grid>();
        foreach (Grid grid in grids)
        {
            Tilemap[] tileMaps = grid.GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tileMap in tileMaps)
            {
                // middle row and same amount on each side
                int halfGridSize = (LevelGridSetting.GridSize - 1) / 2;

                for (int l = -halfGridSize; l <= halfGridSize; l++)
                {
                    for (int h = -halfGridSize; h <= halfGridSize; h++)
                    {
                        Vector3 worldCoordinate = new Vector3(l, h, 0);
                        Vector3Int gridCoordinate = grid.WorldToCell(worldCoordinate);

                        GameObject tileAddonPrefab = LevelGridSetting.GetPrefabFromSprite(tileMap.GetSprite(gridCoordinate));

                        if(tileAddonPrefab != null)
                        {
                            TileAddons.Add(new StartingTileAddonData() { Prefab = conversionSystem.GetPrimaryEntity(tileAddonPrefab), Position = new fix2(x: (fix)worldCoordinate.x, y: (fix)worldCoordinate.y) });
                        }
                    }
                }

                tileMap.gameObject.SetActive(false);
            }

            grid.gameObject.SetActive(false);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (TileAddonsDefinition TileAddonsDef in LevelGridSetting.AddonsDefinition)
        {
            referencedPrefabs.Add(TileAddonsDef.AddonPrefab);
        }
    }
}
