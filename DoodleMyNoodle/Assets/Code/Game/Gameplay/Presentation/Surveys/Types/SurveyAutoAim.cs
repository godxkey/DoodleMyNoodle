using CCC.Fix2D;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;

public class SurveyAutoAim : SurveyBaseController
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private float _trajectoryLength = 8f;

    private Vector2 _instigatorPosition;
    private fix2 _lastLaunchVector;

    private TrajectoryDisplaySystem.TrajectoryHandle _trajectoryDisplay;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Vector
    };

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[] { new GameActionParameterVector.Description() };
    }

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        _trajectoryDisplay = TrajectoryDisplaySystem.Instance.CreateTrajectory();
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Target);

        Update(); // force an update

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        CursorOverlayService.Instance.ResetCursorToDefault();

        result.Add(new GameActionParameterVector.Data(_lastLaunchVector));
        complete();

        yield break;
    }

    private void Update()
    {
        var transform = this.transform;
        var position = transform.position;
        _trajectoryDisplay.Displayed = true;
        _trajectoryDisplay.GravityScale = PresentationHelpers.Surveys.GetProjectileGravityScale(Cache, CurrentContext.UseContext);
        _trajectoryDisplay.Length = _trajectoryLength;
        _trajectoryDisplay.StartPoint = position;
        _trajectoryDisplay.Radius = PresentationHelpers.Surveys.GetProjectileRadiusSetting(Cache, CurrentContext.UseContext);

        _lastLaunchVector = fixMath.Trajectory.SmallestLaunchVelocity((fix)(Cache.PointerWorldPosition.x - position.x), (fix)(Cache.PointerWorldPosition.y - position.y), (fix)(_trajectoryDisplay.GravityScale * GamePresentationCache.Instance.WorldGravity.y));
        _trajectoryDisplay.Velocity = _lastLaunchVector.ToUnityVec();

        CursorOverlayService.Instance.SetCursorColor(_normalColor);
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        CursorOverlayService.Instance.ResetCursorToDefault();
        _trajectoryDisplay.Dispose();
    }
}

