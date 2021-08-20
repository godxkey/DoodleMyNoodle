using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Ladder Animation")]
public class LadderAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Sequence GetDOTWEENAnimationSequence(Sequence sq, Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        sq.Append(spriteTransform.DOLocalRotate(new Vector3(0, 0, 30), Duration / 2, RotateMode.LocalAxisAdd));
        sq.Append(spriteTransform.DOLocalRotate(new Vector3(0, 0, -60), Duration / 2, RotateMode.LocalAxisAdd));
        sq.SetLoops(-1, LoopType.Yoyo);
        return sq;
    }
}