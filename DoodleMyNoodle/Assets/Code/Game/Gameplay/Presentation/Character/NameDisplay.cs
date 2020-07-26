using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class NameDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private TextMeshPro _nameText;

    DirtyValue<Name> _displayName;

    protected override void OnGamePresentationUpdate()
    {
        base.OnGamePresentationUpdate();

        if (SimEntity != Entity.Null && SimWorld.TryGetComponentData(SimEntity, out Name name))
        {
            _displayName.Set(name);
        }
        else
        {
            _displayName.Set(default);
        }

        if (_displayName.ClearDirty())
        {
            _nameText.text = _displayName.Get().ToString();
        }

    }
}