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
        public RuleTile RuleTile;
        public TileMapLayers Layer;
    }

    public List<SpriteLayerData> SpriteLayers = new List<SpriteLayerData>();

    public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
    {
        GameObject newLayer = FindLayerObject(FindLayerFromSelection(GetCurrentSelection(brushTarget, position)));
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

    private TileMapLayers FindLayerFromSelection(Sprite selection)
    {
        if (selection != null)
        {
            foreach (SpriteLayerData spriteLayerData in SpriteLayers)
            {
                if (spriteLayerData.RuleTile?.m_DefaultSprite == selection)
                {
                    return spriteLayerData.Layer;
                }

                if (spriteLayerData.Sprite == selection)
                {
                    return spriteLayerData.Layer;
                }
            }
        }

        return TileMapLayers.Background;
    }

    private Sprite GetCurrentSelection(GameObject brushTarget, BoundsInt position)
    {
        UnityEngine.Tilemaps.Tilemap currentTileMap = brushTarget.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        var currentlySelectedTile = currentTileMap.GetTile(position.position);

        if (currentlySelectedTile is Tile tile)
        {
            return tile.sprite;
        }

        if (currentlySelectedTile is RuleTile ruleTile)
        {
            return ruleTile.m_DefaultSprite;
        }

        return null;
    }
}

[CustomEditor(typeof(SmartBrush))]
public class SmartBrushEditor : GridBrushEditor { }
