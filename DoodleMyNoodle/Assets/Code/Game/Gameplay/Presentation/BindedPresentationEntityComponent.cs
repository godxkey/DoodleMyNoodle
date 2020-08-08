using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class BindedPresentationEntityComponent : GamePresentationBehaviour
{
    private BindedSimEntityManaged _simEntityManaged;

    private BindedSimEntityManaged SimEntityManaged
    {
        get
        {
            if (_simEntityManaged == null)
            {
                _simEntityManaged = GetComponent<BindedSimEntityManaged>();

                if (_simEntityManaged == null)
                {
                    _simEntityManaged = GetComponentInParent<BindedSimEntityManaged>();
                }

                Log.Assert(_simEntityManaged != null, $"{GetType().GetPrettyName()} components should only be placed on presentation GameObjects with a '{nameof(BindedSimEntityManaged)}'");
            }

            return _simEntityManaged;
        }
    }

    public Entity SimEntity => SimEntityManaged == null ? Entity.Null : SimEntityManaged.SimEntity;
}