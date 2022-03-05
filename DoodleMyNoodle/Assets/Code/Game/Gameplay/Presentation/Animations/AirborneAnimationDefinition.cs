using CCC.Fix2D;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Airborne Animation")]
public class AirborneAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();

        int sideSign = 1;
        if (GamePresentationCache.Instance.SimWorld.TryGetComponent(input.SimulationTarget, out PhysicsVelocity velocity))
        {
            if (velocity.Linear.x > 0)
                sideSign *= -1;
        }

        sq.Join(input.PresentationTarget.Bone.DOLocalRotate(new Vector3(0, 0, 360 * sideSign), _duration, RotateMode.LocalAxisAdd));
        return sq;
    }
}
