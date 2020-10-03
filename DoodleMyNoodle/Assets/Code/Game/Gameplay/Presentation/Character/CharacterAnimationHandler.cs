using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class CharacterAnimationHandler : BindedPresentationEntityComponent
{
    public Transform SpriteTransform;

    [Header("Animation Data")]
    public float IdleHeight = 0.05f;
    public float WalkingHeight = 0.08f;

    private Sequence _currentSequence;
    private CommonReads.AnimationTypes _previousState = CommonReads.AnimationTypes.None;

    private Vector3 _spriteStartPos;

    protected override void Awake()
    {
        base.Awake();

        _spriteStartPos = SpriteTransform.localPosition;
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponentData(SimEntity, out AnimationState currentAnimationState))
        {
            AnimationData animationData = SimWorld.GetComponentData<AnimationData>(SimEntity);
            CommonReads.AnimationTypes currentPlayerAnimationState = (CommonReads.AnimationTypes)currentAnimationState.StateID;

            // New Animation State has been apply, play the animation
            if (currentPlayerAnimationState != _previousState)
            {
                _currentSequence.Kill(true);
                _currentSequence = DOTween.Sequence();
                SpriteTransform.localPosition = _spriteStartPos;

                switch (currentPlayerAnimationState)
                {
                    case CommonReads.AnimationTypes.Idle:

                        float idleStartY = _spriteStartPos.y;
                        float idleEndY = idleStartY + IdleHeight;

                        _currentSequence.Append(SpriteTransform.DOLocalMoveY(idleEndY, (float)animationData.TotalDuration / 2).SetEase(Ease.InOutQuad));
                        _currentSequence.Append(SpriteTransform.DOLocalMoveY(idleStartY, (float)animationData.TotalDuration / 2).SetEase(Ease.InOutQuad));
                        _currentSequence.SetLoops(-1);
                        _currentSequence.Goto(UnityEngine.Random.value * _currentSequence.Duration(includeLoops: false), andPlay: true);

                        break;

                    case CommonReads.AnimationTypes.Walking:

                        float walkingStartY = _spriteStartPos.y;
                        float walkingEndY = walkingStartY + WalkingHeight;
                        _currentSequence.Append(SpriteTransform.DOLocalMoveY(walkingEndY, (float)animationData.TotalDuration).SetEase(Ease.InOutQuad));
                        _currentSequence.SetLoops(-1);

                        break;

                    case CommonReads.AnimationTypes.Attack:

                        Vector3 startPos = _spriteStartPos;
                        Vector3 endPos = new Vector3(startPos.x + animationData.Direction.x, startPos.y + animationData.Direction.y, startPos.z);
                        _currentSequence.Append(SpriteTransform.DOLocalMove(endPos, (float)animationData.TotalDuration / 2));
                        _currentSequence.Append(SpriteTransform.DOLocalMove(startPos, (float)animationData.TotalDuration / 2));

                        break;

                    default:
                        break;
                }
            }

            _previousState = currentPlayerAnimationState;
        }
    }
}