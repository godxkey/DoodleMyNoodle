using System;
using UnityEngine;
using UnityEngineX;

[System.Serializable]
public abstract class AnimationDefinition : ScriptableObject
{
    public CommonReads.AnimationTypes AnimationType;
    public float Duration;

    public abstract void TriggerAnimation(Vector3 spriteStartPos, Transform spriteTransform, AnimationData animationData);

    public abstract void InteruptAnimation();
}