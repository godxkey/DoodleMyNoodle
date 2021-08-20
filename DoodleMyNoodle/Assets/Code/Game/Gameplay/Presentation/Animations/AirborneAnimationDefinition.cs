using CCC.Fix2D;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Airborne Animation")]
public class AirborneAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Sequence GetDOTWEENAnimationSequence(Sequence sq, Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        int sideSign = 1;
        if(GamePresentationCache.Instance.SimWorld.TryGetComponent(entity, out PhysicsVelocity velocity)) 
        {
            if (velocity.Linear.x > 0)
                sideSign *= -1;
        }

        sq.Join(spriteTransform.DOLocalRotate(new Vector3(0, 0, 135 * sideSign), Duration, RotateMode.LocalAxisAdd));
        return sq;
    }
}