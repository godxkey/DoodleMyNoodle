using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

public class SurveyWorms : SurveyBaseController
{
    private enum State
    {
        PollingDirection,
        PollingStrength,
        Released,
    }

    [Header("Refs")]
    [SerializeField] private Transform _rotator;
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private Transform _textTransform;
    [SerializeField] private GameObject _preview;
    [SerializeField] private GameObject _arrow;
    [Header("Settings")]
    [SerializeField] private float _strengthVisualScale = 3f;
    [SerializeField] private float _growSpeed = 5f;
    [SerializeField] private float _trajectoryLength;
    [SerializeField] private bool _yoyo = true;
    [SerializeField] private bool _throwWhenReleaseKey = true;
    [SerializeField] private float _minMaxRadius = 0.07f;
    [SerializeField] private bool _allowUserToPickAngle = true;
    [ShowIf(nameof(_allowUserToPickAngle))]
    [SerializeField] private bool _lockAngleAfterPress = true;
    [HideIf(nameof(_allowUserToPickAngle))]
    [SerializeField] private float _forcedAngle = 45;

    private Transform _transform;
    private State _state;
    private float _strength;
    private Vector2 _dir;
    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplay;
    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplayMax;
    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplayMin;

    private void Awake()
    {
        _transform = transform;
    }

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Vector
    };

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        // hide at first
        _arrow.SetActive(false);
        _preview.SetActive(false);
        var timeStarted = Time.unscaledTime;

        var desc = context.GetQueryParam<GameActionParameterVector.Description>();
        // Poll angle
        _state = State.PollingDirection;
        if (_allowUserToPickAngle)
        {
            do
            {
                UpdateDirFromMousePos();
                yield return null;
            } while (!Input.GetMouseButtonDown(0) && context.PressedKey == KeyCode.None);
        }
        else
        {
            _dir = mathX.anglevector(math.radians(_forcedAngle));
            yield return null;
        }

        _trajectoryDisplay = TrajectoryDisplaySystem.Instance.CreateTrajectory();
        if (_minMaxRadius > 0)
        {
            _trajectoryDisplayMax = TrajectoryDisplaySystem.Instance.CreateTrajectory();
            _trajectoryDisplayMin = TrajectoryDisplaySystem.Instance.CreateTrajectory();
            _trajectoryDisplayMin.DisplayPoints = false;
            _trajectoryDisplayMax.DisplayPoints = false;
            _trajectoryDisplayMin.Radius = _minMaxRadius;
            _trajectoryDisplayMax.Radius = _minMaxRadius;
        }

        // Poll strength
        _state = State.PollingStrength;
        _strength = (float)desc.SpeedMin; // start at minimum
        bool grow = true;
        while (!ShouldStop())
        {
            if (_allowUserToPickAngle && !_lockAngleAfterPress)
                UpdateDirFromMousePos();

            _strength = Mathf.MoveTowards(_strength, grow ? (float)desc.SpeedMax : (float)desc.SpeedMin, _growSpeed * Time.deltaTime);
            if (_yoyo && (_strength == (float)desc.SpeedMax || _strength == (float)desc.SpeedMin))
            {
                grow = !grow;
            }
            yield return null;
        }
        bool ShouldStop() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(context.PressedKey) || (_throwWhenReleaseKey && Input.GetKeyUp(context.PressedKey));

        _state = State.Released;

        Vector2 resultVector = _dir * _strength;

        result.Add(new GameActionParameterVector.Data((fix2)resultVector));
        complete();
        yield break;
    }

    private void UpdateDirFromMousePos()
    {
        _dir = (Cache.PointerWorldPosition - (Vector2)_transform.position).normalized;
    }

    // Clean up
    protected override void OnEndSurvey(bool wasCompleted)
    {
        _trajectoryDisplay.Dispose();
        _trajectoryDisplayMax.Dispose();
        _trajectoryDisplayMin.Dispose();
    }

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[]
        {
            new GameActionParameterVector.Description()
            {
                SpeedMax = 8,
                SpeedMin = 0,
            }
        };
    }

    private void Update()
    {
        var desc = CurrentContext.GetQueryParam<GameActionParameterVector.Description>();

        // update visuals
        _preview.SetActive(_state == State.PollingDirection);
        _arrow.SetActive(_state == State.PollingStrength);
        _rotator.rotation = Quaternion.Euler(0, 0, math.degrees(mathX.angle2d(_dir)));
        _arrowContainer.localScale = Vector3.one * _strength * _strengthVisualScale;

        // make sure text is always readable
        _textTransform.rotation = Quaternion.identity;

        if (_trajectoryDisplay.IsValid)
        {
            // trajectory
            Vector2 startOffset = Vector2.zero;
            if (PresentationHelpers.Surveys.GetItemTrajectorySettings(Cache, CurrentContext.UseContext, _dir, out Vector2 offset, out float radius))
            {
                startOffset = offset;
                _trajectoryDisplay.Radius = radius;
            }


            _trajectoryDisplay.GravityScale = PresentationHelpers.Surveys.GetProjectileGravityScale(Cache, CurrentContext.UseContext);
            _trajectoryDisplay.Length = _trajectoryLength;
            _trajectoryDisplay.StartPoint = (Vector2)_transform.position + startOffset;
            _trajectoryDisplay.Velocity = _dir * _strength;

            if (_trajectoryDisplayMin.IsValid)
            {
                _trajectoryDisplayMin.Velocity = _dir * (float)desc.SpeedMin;
                _trajectoryDisplayMin.GravityScale = _trajectoryDisplay.GravityScale;
                _trajectoryDisplayMin.Length = _trajectoryDisplay.Length;
                _trajectoryDisplayMin.StartPoint = _trajectoryDisplay.StartPoint;
            }
            if (_trajectoryDisplayMax.IsValid)
            {
                _trajectoryDisplayMax.Velocity = _dir * (float)desc.SpeedMax;
                _trajectoryDisplayMax.GravityScale = _trajectoryDisplay.GravityScale;
                _trajectoryDisplayMax.Length = _trajectoryDisplay.Length;
                _trajectoryDisplayMax.StartPoint = _trajectoryDisplay.StartPoint;
            }
        }
    }
}
