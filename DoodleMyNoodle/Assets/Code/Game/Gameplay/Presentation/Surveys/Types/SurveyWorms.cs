using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;

public class SurveyWorms : SurveyBaseController
{
    private enum State
    {
        PollingDirection,
        PollingStrength,
        Released,
    }

    [SerializeField] private Transform _rotator;
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private Transform _textTransform;
    [SerializeField] private GameObject _preview;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private float _growSpeed = 5f;
    [SerializeField] private float _strengthVisualScale = 3f;
    [SerializeField] private float _trajectoryLength;

    private Transform _transform;
    private State _state;
    private float _strength;
    private Vector2 _dir;
    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplay;

    private void Awake()
    {
        InfoTextDisplay.Instance.SetText("Choisi une direction et ensuite contr�le la puissance");
        _transform = transform;
    }

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Vector
    };

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        var desc = context.GetQueryParam<GameActionParameterVector.Description>();

        // Poll angle
        _state = State.PollingDirection;
        while (!Input.GetMouseButtonDown(0))
        {
            _dir = (Cache.PointerWorldPosition - (Vector2)_transform.position).normalized;
            yield return null;
        }

        _trajectoryDisplay = TrajectoryDisplaySystem.Instance.CreateTrajectory();

        // Poll strength
        _state = State.PollingStrength;
        _strength = (float)desc.SpeedMin; // start at minimum
        while (Input.GetMouseButton(0))
        {
            _strength = Mathf.MoveTowards(_strength, (float)desc.SpeedMax, _growSpeed * Time.deltaTime);
            yield return null;
        }

        _state = State.Released;

        Vector2 resultVector = _dir * _strength;

        result.Add(new GameActionParameterVector.Data((fix2)resultVector));
        complete();
        yield break;
    }

    // Clean up
    protected override void OnEndSurvey(bool wasCompleted)
    {
        InfoTextDisplay.Instance.ForceHideText();
        _trajectoryDisplay.Dispose();
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
            if (PresentationHelpers.Surveys.TryGetThrowTrajectoryStartOffset(Cache, CurrentContext.UseContext, _dir, out Vector2 offset))
            {
                startOffset = offset;
            }

            _trajectoryDisplay.GravityScale = PresentationHelpers.Surveys.GetProjectileGravityScale(Cache, CurrentContext.UseContext);
            _trajectoryDisplay.Length = _trajectoryLength;
            _trajectoryDisplay.StartPoint = (Vector2)_transform.position + startOffset;
            _trajectoryDisplay.Velocity = _dir * _strength;
        }
    }
}
