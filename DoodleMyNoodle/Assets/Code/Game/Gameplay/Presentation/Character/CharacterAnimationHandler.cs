using CCC.Fix2D;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
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
    private struct DefaultAnimation 
    {
        public AnimationType AnimationType;
        public AnimationDefinition AnimationDefinition;
    }

    [SerializeField] private Transform _bone = null;
    [SerializeField] private float _minimumVelocityThreshold = 0.1f;

    [Header("Animation Data")]
    [FormerlySerializedAs("DefaultAnimations")]
    [SerializeField] private List<DefaultAnimation> _defaultAnimations;

    private class QueuedAnimation
    {
        public List<KeyValuePair<string, object>> Data;
        public AnimationDefinition Definition;
        public bool HasPlayed;
    }

    private List<QueuedAnimation> _queuedAnimations = new List<QueuedAnimation>();

    private AnimationDefinition _currentAnimation;

    private AnimationType _previousState = AnimationType.None;

    private Vector3 _spriteStartPos;
    private Quaternion _spriteStartRot;

    private bool _hasTriggeredAnAnimation = false;
    private fix _lastTransitionTime = -1;

    protected override void Awake()
    {
        base.Awake();

        _spriteStartPos = _bone.localPosition;
        _spriteStartRot = _bone.localRotation;
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
            if (SimWorld.TryGetComponent(SimEntity, out MoveInput input) && SimWorld.TryGetComponent(SimEntity, out ActionPoints ap))
            {
                if (SimWorld.TryGetComponent(SimEntity, out NavAgentFootingState navAgentFootingState))
                {
                    switch (navAgentFootingState.Value)
                    {
                        case NavAgentFooting.Ground:
                            if (input.Value.lengthSquared > (fix)(_minimumVelocityThreshold) && ap.Value > 0)
                            {
                                HandleCharacterMovementAnimation(AnimationType.Walking);
                            }
                            else
                            {
                                HandleCharacterMovementAnimation(AnimationType.Idle);
                            }

                            break;

                        case NavAgentFooting.Ladder:

                            if (input.Value.y == 0 || ap.Value <= 0)
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
        //bool canChangeAnimation = true;
        //if (_currentAnimation != null)
        //{
        //    canChangeAnimation = SimWorld.Time.ElapsedTime >= _lastTransitionTime + (fix)_currentAnimation.Duration;
        //}

        //if (!canChangeAnimation)
        //{
        //    return _hasTriggeredAnAnimation;
        //}

        foreach (GameActionUsedEventData gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            if (gameActionEvent.GameActionContext.FirstInstigatorActor == SimEntity && gameActionEvent.GameActionContext.Action != Entity.Null && !_hasTriggeredAnAnimation)
            {
                TriggerAnimationInteruptionOnStateChange();

                _hasTriggeredAnAnimation = true;
                _lastTransitionTime = SimWorld.Time.ElapsedTime;

                // GAME ACTION AUTH & ANIMATION TRIGGER
                SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Action, out SimAssetId instigatorAssetId);
                GameObject instigatorPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
                if (instigatorPrefab.TryGetComponent(out GameActionAuth gameActionAuth))
                {
                    _currentAnimation = gameActionAuth.Animation;

                    // add additionnal animation to play in queue (skip the first we'll play)
                    if (gameActionAuth.PlayAnimation)
                    {
                        for (int i = 1; i < gameActionEvent.GameActionResult.Count; i++)
                        {
                            // ANIMATION DATA
                            List<KeyValuePair<string, object>> currentAnimationData = new List<KeyValuePair<string, object>>();
                            currentAnimationData.Add(new KeyValuePair<string, object>("GameActionContext", gameActionEvent.GameActionContext));

                            switch (i)
                            {
                                case 1:
                                    currentAnimationData.Add(new KeyValuePair<string, object>("GameActionContextResult", gameActionEvent.GameActionResult.DataElement_1));
                                    break;
                                case 2:
                                    currentAnimationData.Add(new KeyValuePair<string, object>("GameActionContextResult", gameActionEvent.GameActionResult.DataElement_2));
                                    break;
                                case 3:
                                    currentAnimationData.Add(new KeyValuePair<string, object>("GameActionContextResult", gameActionEvent.GameActionResult.DataElement_3));
                                    break;
                                default:
                                    break;
                            }
                            
                            _queuedAnimations.Add(new QueuedAnimation() { Data = currentAnimationData, Definition = _currentAnimation, HasPlayed = false });
                        }
                    }

                    // ANIMATION DATA
                    List<KeyValuePair<string, object>> animationData = new List<KeyValuePair<string, object>>();
                    animationData.Add(new KeyValuePair<string, object>("GameActionContext", gameActionEvent.GameActionContext));
                    animationData.Add(new KeyValuePair<string, object>("GameActionContextResult", gameActionEvent.GameActionResult.DataElement_0));

                    if (_currentAnimation == null)
                    {
                        FindAnimation(AnimationType.GameAction).TriggerAnimation(SimEntity, _spriteStartPos, _bone, animationData);
                    }
                    else if (gameActionAuth.PlayAnimation)
                    {
                        _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, _bone, animationData);
                    }
                }

                _previousState = AnimationType.GameAction;
            }
        }

        if (!_hasTriggeredAnAnimation)
        {
            // find queued animation to play
            QueuedAnimation animationToPlay = null;
            for (int i = 0; i < _queuedAnimations.Count; i++)
            {
                if (!_queuedAnimations[i].HasPlayed)
                {
                    animationToPlay = _queuedAnimations[i];
                    _queuedAnimations[i].HasPlayed = true;
                    break;
                }
            }

            if (animationToPlay != null)
            {
                TriggerAnimationInteruptionOnStateChange();

                _hasTriggeredAnAnimation = true;
                _lastTransitionTime = SimWorld.Time.ElapsedTime;

                _currentAnimation = animationToPlay.Definition;

                if (_currentAnimation == null)
                {
                    FindAnimation(AnimationType.GameAction).TriggerAnimation(SimEntity, _spriteStartPos, _bone, animationToPlay.Data);
                }
                else
                {
                    _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, _bone, animationToPlay.Data);
                }

                _previousState = AnimationType.GameAction;
            }
            else
            {
                _queuedAnimations.Clear();
            }
        }

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
                    anim.TriggerAnimation(SimEntity, _spriteStartPos, _bone, null);
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
                _currentAnimation.TriggerAnimation(SimEntity, _spriteStartPos, _bone, null);
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

        _bone.localPosition = _spriteStartPos;
        _bone.localRotation = _spriteStartRot;
    }

    private AnimationDefinition FindAnimation(AnimationType animationType)
    {
        foreach (DefaultAnimation defaultAnimation in _defaultAnimations)
        {
            if (defaultAnimation.AnimationType == animationType)
            {
                return defaultAnimation.AnimationDefinition;
            }
        }

        return null;
    }
}