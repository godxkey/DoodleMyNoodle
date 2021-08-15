using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[System.Serializable]
public class VFXDefinition : ScriptableObject
{
    public float Duration;
    public GameObject VFXToSpawn;

    protected List<KeyValuePair<string, object>> Data;

    public void TriggerVFX(Entity entity, Transform spriteTransform, List<KeyValuePair<string, object>> vfxData)
    {
        Data = vfxData;

        OnTriggerVFX(entity, spriteTransform);
    }

    protected virtual void OnTriggerVFX(Entity entity, Transform spriteTransform) 
    {
        Instantiate(VFXToSpawn);
    }

    public T GetVFXData<T>(string dataTypeID)
    {
        foreach (KeyValuePair<string, object> data in Data)
        {
            if (data.Key == dataTypeID)
            {
                return (T)data.Value;
            }
        }

        Log.Warning($"VFX Data ({dataTypeID}) couldn't be found in {name}");

        return default;
    }
}