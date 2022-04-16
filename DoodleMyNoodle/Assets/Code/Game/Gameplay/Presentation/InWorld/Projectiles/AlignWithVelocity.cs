using CCC.Fix2D;
using System;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

public class AlignWithVelocity : BindedPresentationEntityComponent
{
    [Range(0, 360)]
    [SerializeField] private float _offset;

    Transform _tr;

    public override void OnGameAwake()
    {
        base.OnGameAwake();
        _tr = transform;
    }

    public override void PresentationUpdate()
    {
        if (SimWorld.TryGetComponent(SimEntity, out PhysicsVelocity velocity))
        {
            float2 velocity2D = (float2)velocity.Linear;

            if (!velocity2D.Equals(float2(0, 0)))
            {
                float angle = degrees(angle2d(velocity2D)) + _offset;
                _tr.rotation = Quaternion.Euler(Vector3.forward * angle);
            }

        }
    }
}