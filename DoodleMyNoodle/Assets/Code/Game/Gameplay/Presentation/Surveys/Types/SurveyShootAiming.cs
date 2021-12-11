using CCC.Fix2D;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;

public class SurveyShootAiming : SurveyBaseController
{
    [SerializeField] private GameObject _rotatingDisplay;
    [SerializeField] private GameObject _weaponPreview;
    [SerializeField] private float _weaponPreviewDisplacementFactor = 1;
    [SerializeField] private Transform _line;

    [SerializeField] private float _aimingAngleMin = -45;
    [SerializeField] private float _aimingAngleMax = 45;
    [SerializeField] private float _aimingSpeed = 0.1f;

    [SerializeField] private float _velocityMult = 3;
    [SerializeField] private float _velocityPow = 1.5f;

    [SerializeField] private float _trajectoryLength = 8f;

    private GameActionParameterVector.Description _vectorDesc;

    private enum ShootAimingState
    {
        SelectingDirection,
        Aiming,
        Shoot
    }

    private ShootAimingState _shootAimingState;
    private Vector2 _shootingVector;
    private float _shootingSpeed;
    private Entity _originEntity;
    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplay;
    private Sequence _rotatingAnimation;

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[] { new GameActionParameterVector.Description() { SpeedMax = 10, SpeedMin = 0 } };
    }

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Vector
    };

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        //Setup
        _trajectoryDisplay = TrajectoryDisplaySystem.Instance.CreateTrajectory();
        CameraMovementController.Instance.SetMaxZoom();
        CameraMovementController.Instance.CenterOnPawn();
        CameraMovementController.Instance.ToggleCameraMovement();

        _weaponPreview.SetActive(false);

        _shootAimingState = ShootAimingState.SelectingDirection;

        _vectorDesc = context.GetQueryParam<GameActionParameterVector.Description>();
        if (_vectorDesc.UsePreviousParameterOriginLocation && context.CurrentData.Count > 0)
        {
            // could fetch all previous to find a valid one
            if (context.CurrentData[context.CurrentData.Count - 1] is GameActionParameterPosition.Data posData)
            {
                transform.position = posData.Position.ToUnityVec();
            }
            else if (context.CurrentData[context.CurrentData.Count - 1] is GameActionParameterEntity.Data entityData)
            {
                _originEntity = entityData.Entity;
                if (SimWorld.TryGetComponent(_originEntity, out FixTranslation fixTranslation))
                {
                    transform.position = fixTranslation.Value.ToUnityVec();
                }
            }
        }

        while (_shootAimingState != ShootAimingState.Aiming)
        {
            UpdateDirection();

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _shootAimingState = ShootAimingState.Aiming;
            }

            yield return null;
        }

        _weaponPreview.SetActive(true);
        SetupAimingRotationAnimation();

        while (_shootAimingState != ShootAimingState.Shoot)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _rotatingAnimation.Kill();
                _shootAimingState = ShootAimingState.Shoot;
            }

            yield return null;
        }

        Vector2 _finalVector = ViewToSimVector((_line.position - transform.position) * 2);
        result.Add(new GameActionParameterVector.Data((fix2)_finalVector));

        complete();
    }

    private void Update()
    {
        UpdateTrajectoryVisuals();
    }

    private void UpdateTrajectoryVisuals()
    {
        Vector2 trajectoryVector = _shootAimingState == ShootAimingState.SelectingDirection ? _shootingVector : ViewToSimVector((_line.position - transform.position) * 2);

        // trajectory
        if (_shootAimingState != ShootAimingState.Shoot)
        {
            Vector2 startOffset = Vector2.zero;
            // custom entity origin (not the item)
            if (_vectorDesc.UsePreviousParameterOriginLocation)
            {
                _trajectoryDisplay.Radius = (float)CommonReads.GetActorRadius(Cache.SimWorld, _originEntity);
            }
            else
            {
                // throwing from the item
                if (PresentationHelpers.Surveys.GetItemTrajectorySettings(Cache, CurrentContext.UseContext, trajectoryVector.normalized, out Vector2 offset, out float radius))
                {
                    startOffset = offset;
                    _trajectoryDisplay.Radius = radius;
                }
            }

            _trajectoryDisplay.GravityScale = PresentationHelpers.Surveys.GetProjectileGravityScale(Cache, CurrentContext.UseContext);
            _trajectoryDisplay.Length = _trajectoryLength;
            _trajectoryDisplay.StartPoint = (Vector2)transform.position + startOffset;
            _trajectoryDisplay.Velocity = trajectoryVector;
        }
    }

    private void UpdateDirection()
    {
        _shootingVector = ViewToSimVector(Cache.PointerWorldPosition - (Vector2)transform.position);
        _shootingSpeed = _shootingVector.magnitude;

        UpdateDirectionDisplay();
    }

    private void UpdateDirectionDisplay() 
    {
        // direction line
        Vector2 _viewShootingVector = SimToViewVector(_shootingVector);
        _line.position = transform.position + ((Vector3)_viewShootingVector / 2);
        _line.localScale = new Vector3(_viewShootingVector.magnitude, 1, 1);
        _line.rotation = Quaternion.Euler(0, 0, math.degrees(mathX.angle2d(_viewShootingVector)));

        // preview weapon
        Vector3 normalizeShootingVector = (_viewShootingVector);
        normalizeShootingVector.Normalize();
        _weaponPreview.transform.position = transform.position + (normalizeShootingVector * _weaponPreviewDisplacementFactor);
        _weaponPreview.transform.rotation = Quaternion.Euler(0, 0, math.degrees(mathX.angle2d(_viewShootingVector)) + -45);
    }

    private void SetupAimingRotationAnimation()
    {
        _rotatingDisplay.transform.rotation = new Vector3(0, 0, _aimingAngleMax).ToEulerRotation();

        _rotatingAnimation = DOTween.Sequence();
        _rotatingAnimation.Join(_rotatingDisplay.transform.DORotate(new Vector3(0, 0, _aimingAngleMin), _aimingSpeed).SetEase(Ease.Linear));
        _rotatingAnimation.Append(_rotatingDisplay.transform.DORotate(new Vector3(0, 0, _aimingAngleMax), _aimingSpeed).SetEase(Ease.Linear));
        _rotatingAnimation.SetLoops(-1, LoopType.Yoyo);
    }

    private Vector2 ViewToSimVector(Vector2 viewVector)
    {
        var v = viewVector;

        // pow
        var length = v.magnitude;
        v = (v / length) * math.pow(length, _velocityPow);

        // mult
        v *= _velocityMult;

        // clamp
        v = mathX.clampLength(v, (float)_vectorDesc.SpeedMin, (float)_vectorDesc.SpeedMax);

        return v;
    }

    private Vector2 SimToViewVector(Vector2 simVelocityVector)
    {
        var v = simVelocityVector;

        // undo mult
        v /= _velocityMult;

        // undo pow
        var length = v.magnitude;
        v = (v / length) * math.pow(length, 1f / _velocityPow);

        return v;
    }

    // Clean up
    protected override void OnEndSurvey(bool wasCompleted)
    {
        _line.gameObject.SetActive(false);
        _trajectoryDisplay.Dispose();
    }
}
