using System;
using UnityEngine;
using UnityEngineX;

public class ActivateFlashOnProximityExplosionActive : BindedPresentationEntityComponent
{
    [SerializeField]
    private AlphaAnimationFlash AlphaAnimationFlash;

    protected override void OnGamePresentationUpdate()
    {
        if(SimWorld.TryGetComponent(SimEntity, out ExplodeOnProximity explodeOnProximity))
        {
            if (AlphaAnimationFlash != null)
            {
                AlphaAnimationFlash.enabled = explodeOnProximity.Activated;
            }
        }
    }
}