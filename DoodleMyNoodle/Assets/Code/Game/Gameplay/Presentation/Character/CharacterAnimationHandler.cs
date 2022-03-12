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
    [SerializeField] private SpriteRenderer _spriteRenderer = null;

    [Header("Animation Data")]
    [FormerlySerializedAs("DefaultAnimations")]
    [SerializeField] private List<DefaultAnimation> _defaultAnimations;

    private readonly static int s_emptyAnimStateHash = Animator.StringToHash("Empty");

    private AnimationType _previousState = AnimationType.None;
    private float _currentAnimationFinishTime = -1;
    private AnimationDefinition _currentAnimation;
    private int _currentAnimationTriggerId = -1;
    private AnimationDefinition.PresentationTarget _presentationTarget;

    private static int s_nextAnimationTriggerId = 0;

    protected override void Awake()
    {
        base.Awake();

        _presentationTarget = new AnimationDefinition.PresentationTarget(gameObject, _bone, _spriteRenderer);

    }

    private void OnDisable()
    {
        SetAnimation(null, null);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (HandleGameActionAnimation())
            return;

        if (HandleDeathAnimation())
            return;

        HandleCharacterMovementAnimation(AnimationType.Idle);
    }

    private bool HandleGameActionAnimation()
    {
        foreach (GameActionUsedEventData gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            if (gameActionEvent.GameActionContext.LastPhysicalInstigator == SimEntity && gameActionEvent.GameActionContext.Action != Entity.Null)
            {
                // GAME ACTION AUTH & ANIMATION TRIGGER
                SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Action, out SimAssetId actionAssetId);
                GameObject actionPrefab = PresentationHelpers.FindSimAssetPrefab(actionAssetId);

                if (actionPrefab != null && actionPrefab.TryGetComponent(out GameActionAuth gameActionAuth) && gameActionAuth.Animation != null)
                {
                    var anim = gameActionAuth.Animation;
                    var parameters = new Dictionary<string, object>();
                    parameters["GameActionContext"] = gameActionEvent.GameActionContext;
                    parameters["GameActionContextResult"] = gameActionEvent.GameActionResult.DataElement_0;
                    _previousState = AnimationType.GameAction;
                    SetAnimation(anim, parameters);
                    return true;
                }
            }
        }
        return false;
    }

    private bool HandleDeathAnimation()
    {
        if (SimWorld.HasComponent<DeadTag>(SimEntity))
        {
            if (_previousState != AnimationType.Death)
            {
                SetAnimation(FindAnimation(AnimationType.Death), parameters: null);

                _previousState = AnimationType.Death;
            }

            return true;
        }

        return false;
    }

    private void HandleCharacterMovementAnimation(AnimationType animation)
    {
        // Whenever previously Triggered Animation is over, we can loop in movement animation
        bool canChangeAnimation = Time.time >= _currentAnimationFinishTime;

        if (canChangeAnimation & _previousState != animation)
        {
            SetAnimation(FindAnimation(animation), parameters: null);

            _previousState = animation;
        }
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

    private void SetAnimation(AnimationDefinition newAnim, Dictionary<string, object> parameters)
    {
        // stop current
        if (_currentAnimation != null)
        {
            _currentAnimation.StopAnimation(new AnimationDefinition.StopInput(_currentAnimationTriggerId, _presentationTarget));
            _currentAnimation = null;
            _currentAnimationFinishTime = -1;
            _currentAnimationTriggerId = -1;
        }

        // reset state
        _bone.localPosition = Vector3.zero;
        _bone.localRotation = Quaternion.identity;

        // Start new anim 
        if (newAnim != null)
        {
            AnimationDefinition.TriggerOuput output = default;
            AnimationDefinition.TriggerInput input = new AnimationDefinition.TriggerInput(SimEntity, _presentationTarget, parameters, s_nextAnimationTriggerId++);
            newAnim.TriggerAnimation(input, ref output);

            _currentAnimationTriggerId = input.TriggerId;
            _currentAnimation = newAnim;
            _currentAnimationFinishTime = Time.time + output.Duration;
        }
    }
}