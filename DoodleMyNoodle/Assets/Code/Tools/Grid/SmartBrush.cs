using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using UnityEditor;

[CreateAssetMenu]
[CustomGridBrush(hideAssetInstances: false, hideDefaultInstance: true, defaultBrush: false, defaultName: "Smart Brush")]
public class SmartBrush : GridBrush
{
    public enum TileMapLayers
    {
        Background,
        LevelContent,
        Foreground
    }

    [System.Serializable]
    public struct SpriteLayerData
    {
        public TileBase Tile;
        public TileMapLayers Layer;
    }

    public List<SpriteLayerData> SpriteLayers = new List<SpriteLayerData>();

    public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
    {
        TileMapLayers? targetLayer = FindLayerFromSelection(GetCurrentSelection(brushTarget, position));
        
        GameObject newLayer = FindLayerObject(targetLayer);

        if (newLayer != null)
        {
            GridPaintingState.scenePaintTarget = newLayer;
        }

        base.Pick(gridLayout, brushTarget, position, pivot);
    }

    private GameObject FindLayerObject(TileMapLayers? layer)
    {
        if (layer == null)
            return null;

        foreach (GameObject target in GridPaintingState.validTargets)
        {
            if (layer.ToString() == target.name)
            {
                return target;
            }
        }

        return null;
    }

    private TileMapLayers? FindLayerFromSelection(TileBase selection)
    {
        if (selection != null)
        {
            foreach (SpriteLayerData spriteLayerData in SpriteLayers)
            {
                if (spriteLayerData.Tile == selection)
                {
                    return spriteLayerData.Layer;
                }
            }
        }

        return null;
    }

    private TileBase GetCurrentSelection(GameObject brushTarget, BoundsInt position)
    {
        Tilemap currentTileMap = brushTarget.GetComponent<Tilemap>();
        return currentTileMap.GetTile(position.position);
    }
}

[CustomEditor(typeof(SmartBrush))]
public class SmartBrushEditor : GridBrushEditor { }
