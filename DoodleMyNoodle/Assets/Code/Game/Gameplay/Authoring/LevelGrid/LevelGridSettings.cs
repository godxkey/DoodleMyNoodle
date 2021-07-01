using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Serializable]
public struct EntitySpriteBinding
{
    public TileBase Tile;
    public GameObject EntityPrefab;
}

[CreateAssetMenu(menuName = "DoodleMyNoodle/Grid/Settings")]
public class LevelGridSettings : ScriptableObject
{
    public List<EntitySpriteBinding> SimEntitySpriteBindings = new List<EntitySpriteBinding>();
    public SimAsset DefaultTerrainTile;
    public SimAsset DefaultLadderTile;

    public GameObject GetSimEntityPrefabFromTile(TileBase tile)
    {
        if (tile != null)
        {
            foreach (EntitySpriteBinding binding in SimEntitySpriteBindings)
            {
                if (binding.Tile == tile)
                {
                    return binding.EntityPrefab;
                }
            }
        }

        return null;
    }
}
