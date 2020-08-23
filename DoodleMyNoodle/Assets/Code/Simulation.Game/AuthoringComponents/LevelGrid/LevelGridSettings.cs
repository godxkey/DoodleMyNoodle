using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct EntitySpriteBinding
{
    public Sprite SpriteVisual;
    [FormerlySerializedAs("AddonsSimulationPrefab")]
    public GameObject EntityPrefab;
}

[CreateAssetMenu(menuName = "DoodleMyNoodle/Grid/Settings")]
public class LevelGridSettings : ScriptableObject
{
    [FormerlySerializedAs("AddonsDefinition")]
    public List<EntitySpriteBinding> SimEntitySpriteBindings = new List<EntitySpriteBinding>();

    public GameObject GetSimEntityPrefabFromSprite(Sprite sprite)
    {
        if(sprite != null)
        {
            foreach (EntitySpriteBinding binding in SimEntitySpriteBindings)
            {
                if (binding.SpriteVisual == sprite)
                {
                    return binding.EntityPrefab;
                }
            }
        }

        return null;
    }
}
