using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all sim 'MonoBehaviour' scripts
/// </summary>
[RequireComponent(typeof(SimEntity))]
public class SimComponent : SimObject
{
    public SimEntity SimEntity
    {
        get
        {
            if (_cachedSimEntity == null)
            {
                _cachedSimEntity = GetComponent<SimEntity>();
            }

            return _cachedSimEntity;
        }
    }
    [NonSerialized]
    SimEntity _cachedSimEntity;
}