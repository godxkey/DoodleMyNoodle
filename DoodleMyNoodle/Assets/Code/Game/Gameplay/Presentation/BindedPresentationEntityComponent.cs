using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class BindedPresentationEntityComponent : GamePresentationBehaviour
{
    private BindedSimEntityManaged _simEntityManaged;

    protected override void OnGamePresentationUpdate()
    {
        if (_simEntityManaged != null)
            return;

        _simEntityManaged = GetComponent<BindedSimEntityManaged>();

        if (_simEntityManaged == null)
        {
            _simEntityManaged = GetComponentInParent<BindedSimEntityManaged>();
        }

        Log.Assert(_simEntityManaged != null, $"{GetType().GetPrettyName()} components should only be placed on presentation GameObjects with a '{nameof(BindedSimEntityManaged)}'");
    }

    public Entity SimEntity => _simEntityManaged == null ? Entity.Null : _simEntityManaged.SimEntity;
}