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

    protected List<KeyValuePair<string, object>> _data;

    public void TriggerVFX(List<KeyValuePair<string, object>> vfxData)
    {
        _data = vfxData;

        OnTriggerVFX();
    }

    public void TriggerVFX(params KeyValuePair<string, object>[] vfxData)
    {
        List<KeyValuePair<string, object>> vfxData_List = new List<KeyValuePair<string, object>>(vfxData);
        _data = vfxData_List;

        OnTriggerVFX();
    }

    protected virtual void OnTriggerVFX() 
    {
        Instantiate(VFXToSpawn);
    }

    protected T GetVFXData<T>(string dataTypeID)
    {
        foreach (KeyValuePair<string, object> data in _data)
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