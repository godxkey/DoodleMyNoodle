
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;

public class SurveyAngryBirds : SurveyBaseController2
{
    [SerializeField] private Transform _center;
    [SerializeField] private Transform _dragTarget;
    [SerializeField] private Transform _line;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private int _velocityFactor = 3;
    [SerializeField] private float _minDragThreshold = 10;

    private enum DragState
    {
        Idle,
        Initialized,
        Dragging,
        Releasing
    }

    private DragState _dragState;
    private Vector2 _dragStartScreenPos;
    private Vector2 _releaseVector;
    private float _releaseSpeed;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Vector
    };

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        Update(); // force first update to avoid visual issue on first frame. We should find a more general fix

        // wait for drag to start
        while (_dragState != DragState.Releasing)
        {
            yield return null;
        }

        // wait for release anim to be complete
        while (!Mathf.Approximately(_dragTarget.position.x, _center.position.x) || !Mathf.Approximately(_dragTarget.position.y, _center.position.y))
        {
            yield return null;
        }

        // submit drag vector
        result.Add(new GameActionParameterVector.Data((fix2)_releaseVector));

        complete();
    }

    private Vector2 GetCurrentVelocityVector()
    {
        return ((Vector2)_center.position - Cache.PointerWorldPosition) * _velocityFactor;
    }

    private void Update()
    {
        UpdateDragLogic();
        UpdateVisuals();
    }

    private void UpdateDragLogic()
    {
        if (Input.GetMouseButton(0))
        {
            if (_dragState == DragState.Idle && Cache.PointedColliders.Contains(_collider))
            {
                _dragStartScreenPos = Input.mousePosition;
                _dragState = DragState.Initialized;
            }

            if (_dragState == DragState.Initialized)
            {
                Vector2 dragScreenVector = (Vector2)Input.mousePosition - _dragStartScreenPos;
                if (dragScreenVector.magnitude > _minDragThreshold * Screen.width / 1920)
                {
                    _dragState = DragState.Dragging;
                }
            }
        }
        else
        {
            if (_dragState == DragState.Initialized)
            {
                _dragState = DragState.Idle;
            }

            if (_dragState == DragState.Dragging)
            {
                _releaseVector = GetCurrentVelocityVector();
                _releaseSpeed = _releaseVector.magnitude;
                _dragState = DragState.Releasing;
            }
        }
    }

    private void UpdateVisuals()
    {
        // drag target
        if (_dragState == DragState.Idle)
        {
            _dragTarget.position = _center.position;
        }
        else if (_dragState == DragState.Dragging || _dragState == DragState.Dragging)
        {
            _dragTarget.position = Cache.PointerWorldPosition;
        }
        else
        {
            _dragTarget.position = Vector3.MoveTowards(_dragTarget.position, _center.position, _releaseSpeed * Time.deltaTime);
        }


        // drag line
        Vector2 dragVector = _center.position - _dragTarget.position;
        _line.position = (_center.position + _dragTarget.position) / 2;
        _line.localScale = new Vector3(dragVector.magnitude, 1, 1);
        _line.rotation = Quaternion.Euler(0, 0, math.degrees(mathX.angle2d(dragVector)));
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        _dragState = DragState.Idle;
    }
}
