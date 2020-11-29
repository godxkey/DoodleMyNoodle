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
    public AnimationDefinition DefaultAnimation;
    public AnimationDefinition DeathAnimation;
    public float IdleHeight = 0.05f;

    private Sequence _currentSequence;
    private AnimationDefinition _currentAnimation;
    private CommonReads.AnimationTypes _previousState = CommonReads.AnimationTypes.None;

    private Vector3 _spriteStartPos;
    private Quaternion _spriteStartRot;

    protected override void Awake()
    {
        base.Awake();

        _spriteStartPos = SpriteTransform.localPosition;
        _spriteStartRot = SpriteTransform.localRotation;
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
                // Interupt and restart
                _currentSequence.Kill(true);
                _currentSequence = DOTween.Sequence();
                if (_currentAnimation != null)
                {
                    _currentAnimation.InteruptAnimation();
                }
                SpriteTransform.localPosition = _spriteStartPos;
                SpriteTransform.localRotation = _spriteStartRot;

                // Trigger animation by anim type
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
                    case CommonReads.AnimationTypes.GameAction:

                        if (animationData.GameActionEntity != Entity.Null)
                        {
                            SimWorld.TryGetComponentData(animationData.GameActionEntity, out SimAssetId instigatorAssetId);
                            GameObject instigatorPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
                            if (instigatorPrefab.TryGetComponent(out GameActionIdAuth gameActionAuth))
                            {
                                _currentAnimation = gameActionAuth.Animation;
                                if (gameActionAuth.PlayAnimation && _currentAnimation != null)
                                {
                                    _currentAnimation.TriggerAnimation(_spriteStartPos, SpriteTransform, animationData);
                                }
                                else if (gameActionAuth.PlayAnimation && DefaultAnimation != null)
                                {
                                    _currentAnimation = DefaultAnimation;
                                    _currentAnimation.TriggerAnimation(_spriteStartPos, SpriteTransform, animationData);
                                }
                                
                                break;
                            }
                        }

                        break;

                    case CommonReads.AnimationTypes.Death:

                        _currentAnimation = DeathAnimation;
                        _currentAnimation.TriggerAnimation(_spriteStartPos, SpriteTransform, animationData);

                        break;

                    default:
                        break;
                }
            }

            _previousState = currentPlayerAnimationState;
        }
    }
}