using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class LevelGridAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public LevelGridSettings LevelGridSetting;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Lets get all tilemaps and spawn whatever was drawn on them

        Grid[] grids = gameObject.GetComponentsInChildren<Grid>();
        foreach (Grid grid in grids)
        {
            Tilemap tileMap = grid.GetComponentInChildren<Tilemap>();
            if(tileMap != null)
            {
                // middle row and same amount on each side
                int halfGridSize = (CreateLevelGrid.GRID_MAX_SIZE - 1) / 2;

                for (int l = -halfGridSize; l <= halfGridSize; l++)
                {
                    for (int h = -halfGridSize; h <= halfGridSize; h++)
                    {
                        Vector3 worldCoordinate = new Vector3(l, h, 0);
                        Vector3Int gridCoordinate = grid.WorldToCell(worldCoordinate);

                        GameObject tileAddonPrefab = LevelGridSetting.GetPrefabFromSprite(tileMap.GetSprite(gridCoordinate));
                        // TODO - KEEP A BUFFER WITH THIS PREFAB
                    }
                }
            }
        }
    }
}
