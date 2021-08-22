using CCC.Fix2D;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class CharacterAnimationHandler : BindedPresentationEntityComponent
{
    public enum AnimationType
    {
        None,
        Idle,
        Walking,
        Ladder,
        AirControl,
        Death,
        Jump,
        GameAction
    }

    [System.Serializable]
    public struct DefaultAnimation 
    {
        public AnimationType AnimationType;
        public AnimationDefinition AnimationDefinition;
    }

    [SerializeField] private Transform SpriteTransform;
    [SerializeField] private float MinimumVelocityThreshold = 0.1f;

    [Header("Animation Data")]
    [SerializeField] private List<DefaultAnimation> DefaultAnimations;

    private AnimationDefinition _currentAnimation;

    private AnimationType _previousState = AnimationType.None;

    private Vector3 _spriteStartPos;
    private Quaternion _spriteStartRot;

    private bool _hasTriggeredAnAnimation = false;
    private fix _lastTransitionTime = -1;

    protected override void Awake()
    {
        base.Awake();

        _spriteStartPos = SpriteTransform.localPosition;
        _spriteStartRot = SpriteTransform.localRotation;
    }

    protected override void OnGamePresentationUpdate()
    {
        _hasTriggeredAnAnimation = false;

        if (!_hasTriggeredAnAnimation) 
        {
            if (HandleGameActionAnimation())
            {
                return;
            }
        }

        if (!_hasTriggeredAnAnimation) 
        {
            if (HandleDeathAnimation())
            {
                return;
            }
        }

        if (!_hasTriggeredAnAnimation) 
        {
            if (SimWorld.TryGetComponent(SimEntity, out MoveInput input) && SimWorld.TryGetComponent(SimEntity, out MoveEnergy energy))
            {
                if (SimWorld.TryGetComponent(SimEntity, out NavAgentFootingState navAgentFootingState))
                {
                    switch (navAgentFootingState.Value)
                    {
                        case NavAgentFooting.Ground:

                            if (input.Value.lengthSquared > (fix)(MinimumVelocityThreshold) && energy.Value > 0)
                            {
                                HandleCharacterMovementAnimation(AnimationType.Walking);
                            }
                            else
                            {
                                HandleCharacterMovementAnimation(AnimationType.Idle);
                            }

                            break;

                        case NavAgentFooting.Ladder:

                            if (input.Value.y == 0 || energy.Value <= 0)
                            {
                                HandleCharacterMovementAnimation(AnimationType.Idle);
                            }
                            else
                            {
                                HandleCharacterMovementAnimation(AnimationType.Ladder);
                            }

                            break;
                        case NavAgentFooting.AirControl:

                            HandleCharacterMovementAnimation(AnimationType.AirControl);
                            break;

                        case NavAgentFooting.None:

                            HandleCharacterMovementAnimation(AnimationType.Jump);
                            break;

                        default:

                            HandleCharacterMovementAnimation(AnimationType.Idle);
                            break;
                    }

                    return;
                }
            }

            HandleCharacterMovementAnimation(AnimationType.Idle);
        }
    }

    private bool HandleGameActionAnimation()
    {
        // Wait until previous Game Action Animation is done
        bool canChangeAnimation = true;
        if (_currentAnimation != null)
        {
            canChangeAnimation = SimWorld.Time.ElapsedTime >= _lastTransitionTime + (fix)_currentAnimation.Duration;
        }

        if (!canChangeAnimation)
        {
            return _hasTriggeredAnAnimation;
        }

        SimWorld.Entities.ForEach((ref GameActionEventData gameActionEvent) =>
        {
            if (gameActionEvent.GameActionContext.InstigatorPawn == SimEntity && gameActionEvent.GameActionContext.Item != Entity.Null && !_hasTriggeredAnAnimation)
            {
                TriggerAnimationInteruptionOnStateChange();

                _hasTriggeredAnAnimation = true;
                _lastTransitionTime = SimWorld.Time.ElapsedTime;

                // ANIMATION DATA
                List<KeyValuePair<string, object>> animationData = new List<KeyValuePair<string, object>>();
                animationData.Add(new KeyValuePair<string, object>("GameActionContext", gameActionEvent.GameActionContext));
                animationData.Add(new KeyValuePair<string, object>("AttackVector", (Vector2)gameActionEvent.GameActionResult.AttackVector));

                // ITEM AUTH & ANIMATION TRIGGER
                SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Item, out SimAssetId instigatorAssetId);
                GameObject instigatorPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
                if (instigatorPrefab.TryGetComponent(out ItemAuth gameActionAuth))
                {
                    _currentAnimation = gameActionAuth.Animation;
                    if (_currentAnimation == null)
                    {
                        FindAnimation(AnimationType.GameAction).TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
                    }
                    else if (gameActionAuth.PlayAnimation)
                    {
                        _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
                    }
                }

                _previousState = AnimationType.GameAction;
            }
        });

        return _hasTriggeredAnAnimation;
    }

    private bool HandleDeathAnimation() 
    {
        if (SimWorld.HasComponent<DeadTag>(SimEntity))
        {
            if (_previousState != AnimationType.Death)
            {
                TriggerAnimationInteruptionOnStateChange();

                _hasTriggeredAnAnimation = true;
                _lastTransitionTime = SimWorld.Time.ElapsedTime;

                AnimationDefinition anim = FindAnimation(AnimationType.Death);
                if (anim != null)
                {
                    anim.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, null);
                }

                _previousState = AnimationType.Death;
            }

            return true;
        }

        return false;
    }

    private void HandleCharacterMovementAnimation(AnimationType animation)
    {
        // Whenever previously Triggered Animation is over, we can loop in movement animation
        bool canChangeAnimation = true;
        if (_currentAnimation != null)
        {
            canChangeAnimation = SimWorld.Time.ElapsedTime >= _lastTransitionTime + (fix)_currentAnimation.Duration;
        }

        if (canChangeAnimation & _previousState != animation)
        {
            TriggerAnimationInteruptionOnStateChange();

            _currentAnimation = FindAnimation(animation);
            if (_currentAnimation != null)
            {
                _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, null);
            }

            _previousState = animation;
        }
    }

    private void TriggerAnimationInteruptionOnStateChange()
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.InteruptAnimation(SimEntity);
        }

        SpriteTransform.localPosition = _spriteStartPos;
        SpriteTransform.localRotation = _spriteStartRot;
    }

    private AnimationDefinition FindAnimation(AnimationType animationType)
    {
        foreach (DefaultAnimation defaultAnimation in DefaultAnimations)
        {
            if (defaultAnimation.AnimationType == animationType)
            {
                return defaultAnimation.AnimationDefinition;
            }
        }

        return null;
    }
}