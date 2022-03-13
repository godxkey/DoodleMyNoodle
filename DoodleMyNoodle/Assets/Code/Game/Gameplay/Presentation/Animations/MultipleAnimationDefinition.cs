using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Multiple Animation")]
public class MultipleAnimationDefinition : AnimationDefinition
{
    [SerializeField] private List<AnimationDefinition> _animations;

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput ouput)
    {
        foreach (var animation in _animations)
        {
            animation.TriggerAnimation(input, ref ouput);
        }
    }

    public override void StopAnimation(StopInput input)
    {
        foreach (var animation in _animations)
        {
            animation.StopAnimation(input);
        }
    }
}