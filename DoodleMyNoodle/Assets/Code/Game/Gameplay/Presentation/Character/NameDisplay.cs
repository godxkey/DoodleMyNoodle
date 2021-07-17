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

    private float _fadeDelayTimer;

    [SerializeField] private float fadeSpeed = 4;

    protected override void OnGamePresentationUpdate()
    {
        if (SimEntity != Entity.Null && SimWorld.TryGetComponent(SimEntity, out Name name))
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
            Show(5);
        }
        if (Cache.PointerInWorld && Cache.LocalPawn != Entity.Null)
        {
            foreach (var entity in Cache.PointedBodies)
            {
                if (entity == SimEntity)
                {
                    Show(0.4f);
                }
            }
        }
    }

    private void Update()
    {
        _fadeDelayTimer -= Time.deltaTime;

        if (_fadeDelayTimer <= 0)
        {
            _nameText.alpha -= (Time.deltaTime * fadeSpeed);
        }
    }

    public void Show(float fadeDelay)
    {
        _nameText.alpha = 1;
        _fadeDelayTimer = Mathf.Max(fadeDelay, _fadeDelayTimer);
    }
}