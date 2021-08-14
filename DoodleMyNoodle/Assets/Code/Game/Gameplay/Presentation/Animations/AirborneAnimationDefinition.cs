using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Airborne Animation")]
public class AirborneAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Sequence GetDOTWEENAnimationSequence(Sequence sq, Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        sq.Append(spriteTransform.DOLocalRotate(new Vector3(0, 0, 360), Duration, RotateMode.LocalAxisAdd));
        sq.SetLoops(-1, LoopType.Incremental);
        return sq;
    }
}