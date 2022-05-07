using DG.Tweening;
using System;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Transform Sequence Animation")]
public class TransformSequenceAnimation : DOTWEENAnimationDefinition
{
    [Serializable]
    private class Operation
    {
        public enum PropertyType
        {
            Position,
            Rotation,
            Scale
        }

        public float Time;
        public PropertyType Type = PropertyType.Position;
        public Vector3 Destination;
        public float Duration;
        public Ease Ease = Ease.Linear;
    }

    [SerializeField] private Operation[] _operations;

    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        var sequence = DOTween.Sequence();

        foreach (var item in _operations)
        {
            Tween tween = null;
            switch (item.Type)
            {
                case Operation.PropertyType.Position:
                    tween = input.PresentationTarget.Bone.DOMove(item.Destination, item.Duration).SetEase(item.Ease);
                    break;
                case Operation.PropertyType.Rotation:
                    tween = input.PresentationTarget.Bone.DORotate(item.Destination, item.Duration).SetEase(item.Ease);
                    break;
                case Operation.PropertyType.Scale:
                    tween = input.PresentationTarget.Bone.DOScale(item.Destination, item.Duration).SetEase(item.Ease);
                    break;
            }

            if (tween == null)
                continue;

            sequence.Insert(item.Time, tween);
        }

        return sequence;
    }
}