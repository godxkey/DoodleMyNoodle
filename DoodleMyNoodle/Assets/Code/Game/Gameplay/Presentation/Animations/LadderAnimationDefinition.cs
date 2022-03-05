using DG.Tweening;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Ladder Animation")]
public class LadderAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(input.PresentationTarget.Bone.DOLocalRotate(new Vector3(0, 0, 30), _duration / 2, RotateMode.LocalAxisAdd));
        sq.Append(input.PresentationTarget.Bone.DOLocalRotate(new Vector3(0, 0, -60), _duration / 2, RotateMode.LocalAxisAdd));
        sq.SetLoops(-1, LoopType.Yoyo);
        return sq;
    }
}