using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class KamikazeDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private GameObject _kamikazeVisual;

    public override void PresentationUpdate()
    {
        if (SimEntity != Entity.Null && SimWorld.TryGetComponent(SimEntity, out ExplodeOnDeath explodeOnDeath))
        {
            _kamikazeVisual.SetActive(!explodeOnDeath.HasExploded);
        }
        else
        {
            _kamikazeVisual.SetActive(false);
        }
    }
}