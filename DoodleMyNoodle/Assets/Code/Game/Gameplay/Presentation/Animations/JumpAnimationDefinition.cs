using CCC.Fix2D;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Jump Animation")]
public class JumpAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();

        int sideSign = 1;
        if (GamePresentationCache.Instance.SimWorld.TryGetComponent(entity, out PhysicsVelocity velocity))
        {
            if (velocity.Linear.x > 0)
                sideSign *= -1;
        }

        sq.Join(spriteTransform.DOLocalRotate(new Vector3(0, 0, 360 * sideSign), Duration, RotateMode.LocalAxisAdd));
        sq.SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        return sq;
    }
}