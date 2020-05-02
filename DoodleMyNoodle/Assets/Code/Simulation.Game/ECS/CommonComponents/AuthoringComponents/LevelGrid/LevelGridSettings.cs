using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TileAddonsDefinition
{
    public Sprite SpriteVisual;
    public GameObject AddonPrefab;
}

[CreateAssetMenu(menuName = "DoodleMyNoodle/Grid/Settings")]
public class LevelGridSettings : ScriptableObject
{
    public List<TileAddonsDefinition> AddonsDefinition = new List<TileAddonsDefinition>();

    public GameObject GetPrefabFromSprite(Sprite sprite)
    {
        foreach (TileAddonsDefinition tileAddon in AddonsDefinition)
        {
            if(tileAddon.SpriteVisual == sprite)
            {
                return tileAddon.AddonPrefab;
            }
        }

        return null;
    }
}
