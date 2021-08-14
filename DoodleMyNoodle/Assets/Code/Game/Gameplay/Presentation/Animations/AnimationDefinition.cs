using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[System.Serializable]
public abstract class AnimationDefinition : ScriptableObject
{
    public float Duration;
    public CharacterAnimationHandler.AnimationType Type;

    protected List<KeyValuePair<string, object>> Data;

    public void TriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform, List<KeyValuePair<string, object>> animationData)
    {
        Data = animationData;

        OnTriggerAnimation(entity, spriteStartPos, spriteTransform);
    }

    public abstract void OnTriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform);

    public abstract void InteruptAnimation(Entity entity);

    public T GetAnimationData<T>(string dataTypeID)
    {
        foreach (KeyValuePair<string, object> data in Data)
        {
            if (data.Key == dataTypeID)
            {
                return (T)data.Value;
            }
        }

        Log.Warning($"Animation Data ({dataTypeID}) couldn't be found in {name}");

        return default;
    }
}