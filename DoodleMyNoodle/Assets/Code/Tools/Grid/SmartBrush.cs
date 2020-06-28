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
        public Sprite Sprite;
        public TileMapLayers Layer;
    }

    public List<SpriteLayerData> SpriteLayers = new List<SpriteLayerData>();

    public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
    {
        GameObject newLayer = FindLayerObject(FindLayerFromSprite(GetCurrentSelectedSprite(brushTarget, position)));
        if (newLayer != null)
        {
            GridPaintingState.scenePaintTarget = newLayer;
        }

        base.Pick(gridLayout, brushTarget, position, pivot);
    }

    private GameObject FindLayerObject(TileMapLayers layer)
    {
        foreach (GameObject target in GridPaintingState.validTargets)
        {
            string test = layer.ToString();
            if (layer.ToString() == target.name)
            {
                return target;
            }
        }

        return null;
    }

    private TileMapLayers FindLayerFromSprite(Sprite sprite)
    {
        foreach (SpriteLayerData spriteLayerData in SpriteLayers)
        {
            if (spriteLayerData.Sprite == sprite)
            {
                return spriteLayerData.Layer;
            }
        }

        Debug.LogError("Sprite not found for SmartBrush");
        return TileMapLayers.Background;
    }

    private Sprite GetCurrentSelectedSprite(GameObject brushTarget, BoundsInt position)
    {
        UnityEngine.Tilemaps.Tilemap currentTileMap = brushTarget.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        Tile currentlySelectedTile = currentTileMap.GetTile(position.position) as Tile;
        return currentlySelectedTile.sprite;
    }
}

[CustomEditor(typeof(SmartBrush))]
public class SmartBrushEditor : GridBrushEditor { }
