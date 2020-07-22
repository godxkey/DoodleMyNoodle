using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class NameDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private TextMeshPro _nameText;

    protected override void OnGamePresentationUpdate()
    {
        base.OnGamePresentationUpdate();

        if (SimEntity != Entity.Null && SimWorld.HasComponent<Name>(SimEntity))
        {
            _nameText.text = SimWorld.GetComponentData<Name>(SimEntity).Value.ToString();
        }
    }
}