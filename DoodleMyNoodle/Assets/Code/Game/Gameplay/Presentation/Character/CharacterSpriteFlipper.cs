using CCC.Fix2D;
using System;
using UnityEngine;
using UnityEngineX;

public class CharacterSpriteFlipper : BindedPresentationEntityComponent
{
    [SerializeField] private Transform _bone = null;
    [SerializeField] private bool _spriteLookingRight = true;

    protected override void OnGamePresentationUpdate()
    {
        PhysicsVelocity velocity = SimWorld.GetComponent<PhysicsVelocity>(SimEntity);

        bool spriteLookingRight = _spriteLookingRight;
        if (SimWorld.HasComponent<DoodleStartDirection>(SimEntity))
        {
            spriteLookingRight = SimWorld.GetComponent<DoodleStartDirection>(SimEntity).IsLookingRight;
        }

        int lookDir = 0;

        if (velocity.Linear.x > (fix)0.05f)
        {
            lookDir = 1;
        }
        else if (velocity.Linear.x < -(fix)0.05f)
        {
            lookDir = -1;
        }

        if (lookDir != 0)
        {
            Quaternion rot = _bone.localRotation;
            Vector3 eulerAngles = rot.eulerAngles;

            eulerAngles.y = (lookDir == 1 ^ spriteLookingRight) ? 180f : 0f;

            rot.eulerAngles = eulerAngles;
            _bone.localRotation = rot;
        }
    }
}