using CCC.Fix2D;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CharacterAnimationHandler : BindedPresentationEntityComponent
{
    public enum AnimationType
    {
        None,
        Idle,
        Walking,
        Ladder,
        Aiborne,
        Death,
        GameAction
    }

    [SerializeField] private Transform SpriteTransform;
    [SerializeField] private float MinimumVelocityThreshold = 0.1f;

    [Header("Animation Data")]
    [SerializeField] private List<AnimationDefinition> DefaultAnimations;

    private AnimationDefinition _currentAnimation;

    private AnimationType _previousState = AnimationType.None;

    private Vector3 _spriteStartPos;
    private Quaternion _spriteStartRot;

    private bool hasTriggeredAnAnimation = false;
    private fix LastTransitionTime = -1;

    protected override void Awake()
    {
        base.Awake();

        _spriteStartPos = SpriteTransform.localPosition;
        _spriteStartRot = SpriteTransform.localRotation;
    }

    protected override void OnGamePresentationUpdate()
    {
        hasTriggeredAnAnimation = false;

        List<KeyValuePair<string, object>> animationData = new List<KeyValuePair<string, object>>();

        if (!hasTriggeredAnAnimation) 
        {
            if (HandleGameActionAnimation(animationData))
            {
                return;
            }
        }

        if (!hasTriggeredAnAnimation) 
        {
            if (HandleDeathAnimation(animationData))
            {
                return;
            }
        }

        if (!hasTriggeredAnAnimation) 
        {
            if (SimWorld.TryGetComponent(SimEntity, out MoveInput input) && SimWorld.TryGetComponent(SimEntity, out MoveEnergy energy))
            {
                if ((input.Value.lengthSquared > (fix)MinimumVelocityThreshold) && (energy.Value > 0))
                {
                    if (SimWorld.TryGetComponent(SimEntity, out NavAgentFootingState navAgentFootingState))
                    {
                        switch (navAgentFootingState.Value)
                        {
                            case NavAgentFooting.Ground:
                                HandleCharacterMovementAnimation(AnimationType.Walking, animationData);
                                break;
                            case NavAgentFooting.Ladder:
                                HandleCharacterMovementAnimation(AnimationType.Ladder, animationData);
                                break;
                            case NavAgentFooting.AirControl:
                                HandleCharacterMovementAnimation(AnimationType.Aiborne, animationData);
                                break;
                            case NavAgentFooting.None:
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    HandleCharacterMovementAnimation(AnimationType.Idle, animationData);
                }
            }
        }
    }

    private bool HandleGameActionAnimation(List<KeyValuePair<string, object>> animationData)
    {
        SimWorld.Entities.ForEach((ref GameActionEventData gameActionEvent) =>
        {
            if (gameActionEvent.GameActionContext.InstigatorPawn == SimEntity && gameActionEvent.GameActionContext.Item != Entity.Null)
            {
                TriggerAnimationInteruptionOnStateChange();

                hasTriggeredAnAnimation = true;
                LastTransitionTime = SimWorld.Time.ElapsedTime;

                // ANIMATION DATA
                animationData.Add(new KeyValuePair<string, object>("GameActionContext", gameActionEvent.GameActionContext));
                animationData.Add(new KeyValuePair<string, object>("AttackVector", (Vector2)gameActionEvent.GameActionResult.AttackVector));

                // ITEM AUTH & ANIMATION TRIGGER
                SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Item, out SimAssetId instigatorAssetId);
                GameObject instigatorPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
                if (instigatorPrefab.TryGetComponent(out ItemAuth gameActionAuth))
                {
                    _currentAnimation = gameActionAuth.Animation;
                    if (_currentAnimation != null)
                    {
                        FindAnimation(AnimationType.GameAction).TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
                    }

                    if (gameActionAuth.PlayAnimation)
                    {
                        _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
                    }
                }

                _previousState = AnimationType.GameAction;
            }
        });

        return hasTriggeredAnAnimation;
    }

    private bool HandleDeathAnimation(List<KeyValuePair<string, object>> animationData) 
    {
        if (SimWorld.HasComponent<DeadTag>(SimEntity))
        {
            if (_previousState != AnimationType.Death)
            {
                TriggerAnimationInteruptionOnStateChange();

                hasTriggeredAnAnimation = true;
                LastTransitionTime = SimWorld.Time.ElapsedTime;

                AnimationDefinition anim = FindAnimation(AnimationType.Death);
                if (anim != null)
                {
                    anim.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
                }

                _previousState = AnimationType.Death;
            }

            return true;
        }

        return false;
    }

    private void HandleCharacterMovementAnimation(AnimationType animation, List<KeyValuePair<string, object>> animationData)
    {
        // Whenever previously Triggered Animation is over, we can loop in movement animation
        bool canChangeAnimation = true;
        if (_currentAnimation != null)
        {
            canChangeAnimation = SimWorld.Time.ElapsedTime >= LastTransitionTime + (fix)_currentAnimation.Duration;
        }

        if (canChangeAnimation & _previousState != animation)
        {
            TriggerAnimationInteruptionOnStateChange();

            _currentAnimation = FindAnimation(animation);
            if (_currentAnimation != null)
            {
                _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, SpriteTransform, animationData);
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
        foreach (AnimationDefinition defaultAnimation in DefaultAnimations)
        {
            if (defaultAnimation.Type == animationType)
            {
                return defaultAnimation;
            }
        }

        return null;
    }
}