using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TileAddonsDefinition
{
    public Sprite SpriteVisual;
    public GameObject AddonSimulationPrefab;
}

[CreateAssetMenu(menuName = "DoodleMyNoodle/Grid/Settings")]
public class LevelGridSettings : ScriptableObject
{
    public int GridSize = 11;

    public List<TileAddonsDefinition> AddonsDefinition = new List<TileAddonsDefinition>();

    public GameObject GetSimEntityPrefabFromSprite(Sprite sprite)
    {
        if(sprite != null)
        {
            foreach (TileAddonsDefinition tileAddon in AddonsDefinition)
            {
                if (tileAddon.SpriteVisual == sprite)
                {
                    return tileAddon.AddonSimulationPrefab;
                }
            }
        }

        return null;
    }
}
