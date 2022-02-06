using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;
using System;
using UnityEngineX;

public class SurveyEntity : SurveyBaseController
{
    [SerializeField] private HighlightService.Params _highlightSettings;

    [SerializeField] private Color _outOfRangeColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Transform _rangeIndicator1;
    [SerializeField] private Transform _rangeIndicator2;
    [SerializeField] private float _rangeIndicatorScaleDiff = 0.1f;

    private Entity? _selectedEntity;
    private DirtyRef<BindedSimEntityManaged> _hoveredEntity = default;
    private Vector2 _instigatorPosition;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Entity
    };

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, System.Action complete, System.Action cancel)
    {
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Target);

        while (_selectedEntity == null)
        {
            yield return null;
        }

        if (Cache.SimWorld.Exists(_selectedEntity.Value))
        {
            result.Add(new GameActionParameterEntity.Data(_selectedEntity.Value));
            complete();
        }
        else
        {
            cancel();
        }
    }

    private void Update()
    {
        _hoveredEntity.Set(FindHoveredEntity());

        if (Input.GetMouseButtonDown(0) && _hoveredEntity.Get() != null)
        {
            _selectedEntity = _hoveredEntity.Get().SimEntity;
            _hoveredEntity.Set(null);
        }

        // update higlight
        if (_hoveredEntity.ClearDirty())
        {
            if (_hoveredEntity.GetPrevious())
            {
                var sprRenderer = _hoveredEntity.GetPrevious().GetComponentInChildren<SpriteRenderer>();
                if (sprRenderer)
                    HighlightService.StopHighlight(sprRenderer);
            }

            if (_hoveredEntity.Get())
            {
                var sprRenderer = _hoveredEntity.Get().GetComponentInChildren<SpriteRenderer>();
                if (sprRenderer)
                    HighlightService.HighlightSprite(sprRenderer, _highlightSettings);
            }
        }

        UpdateRangeFeedback();
    }
    
    private void UpdateRangeFeedback()
    {
        if (SimWorld.TryGetComponent(CurrentContext.Instigator, out FixTranslation position))
        {
            _instigatorPosition = (Vector2)position.Value;
        }

        var paramEntity = CurrentContext.GetQueryParam<GameActionParameterEntity.Description>();

        float range = paramEntity == null ? float.MaxValue : (float)paramEntity.RangeFromInstigator;

        bool inRange = (Cache.PointerWorldPosition - _instigatorPosition).magnitude < range;

        if (inRange)
        {
            CursorOverlayService.Instance.SetCursorColor(_normalColor);
        }
        else
        {
            CursorOverlayService.Instance.SetCursorColor(_outOfRangeColor);
        }

        if (range < 1000)
        {
            _rangeIndicator1.gameObject.SetActive(true);
            _rangeIndicator2.gameObject.SetActive(true);
            _rangeIndicator1.localScale = Vector3.one * range * 2f;
            _rangeIndicator2.localScale = _rangeIndicator1.localScale + Vector3.one * _rangeIndicatorScaleDiff * Cache.CameraSize;
        }
        else
        {
            _rangeIndicator1.gameObject.SetActive(false);
            _rangeIndicator2.gameObject.SetActive(false);
        }
    }

    private BindedSimEntityManaged FindHoveredEntity()
    {
        for (int i = 0; i < Cache.PointedViewEntities.Count; i++)
        {
            if (VerifyEntityForParameters(Cache.PointedViewEntities[i].SimEntity))
            {
                return Cache.PointedViewEntities[i];
            }
        }
        return null;
    }

    private bool VerifyEntityForParameters(Entity entity)
    {
        GameActionParameterEntity.Description paramDescription = CurrentContext.GetQueryParam<GameActionParameterEntity.Description>();

        if (!paramDescription.IncludeSelf && entity == Cache.LocalPawn)
        {
            return false;
        }

        if (paramDescription.RequiresAttackableEntity)
        {
            if (!Cache.SimWorld.HasComponent<Health>(entity))
            {
                return false;
            }
        }

        if (paramDescription.CustomPredicate != null)
        {
            if (paramDescription.CustomPredicate(SimWorld, entity) == false)
                return false;
        }

        fix2 targetPos = Cache.SimWorld.GetComponent<FixTranslation>(entity);
        fix squaredDistance = fixMath.distancesq(Cache.LocalPawnPosition, targetPos);
        if (squaredDistance > paramDescription.RangeFromInstigator * paramDescription.RangeFromInstigator)
        {
            return false;
        }

        return true;
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        var sprRenderer = _hoveredEntity.Get()?.GetComponentInChildren<SpriteRenderer>();
        if (sprRenderer)
            HighlightService.StopHighlight(sprRenderer);

        CursorOverlayService.Instance.ResetCursorToDefault();
        _selectedEntity = null;
    }

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[]
        {
            new GameActionParameterEntity.Description() { RangeFromInstigator = 5 }
        };
    }
}
