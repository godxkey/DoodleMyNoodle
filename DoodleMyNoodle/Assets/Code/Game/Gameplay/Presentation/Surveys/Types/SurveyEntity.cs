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

    private Entity? _selectedEntity;
    private DirtyRef<BindedSimEntityManaged> _hoveredEntity;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.Entity
    };

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        while (_selectedEntity == null)
        {
            yield return null;
        }

        if (Cache.SimWorld.Exists(_selectedEntity.Value))
        {
            result.Add(new GameActionParameterEntity.Data(Cache.SimWorld.GetComponentData<FixTranslation>(_selectedEntity.Value)));
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
        GameActionParameterEntity.Description paramDescription = (GameActionParameterEntity.Description)QueryParameters[0];

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

        fix2 targetPos = Cache.SimWorld.GetComponentData<FixTranslation>(entity);
        fix squaredDistance = fixMath.distancesq(Cache.LocalPawnPosition, targetPos);
        if (squaredDistance > paramDescription.RangeFromInstigator * paramDescription.RangeFromInstigator)
        {
            return false;
        }

        return true;
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        _selectedEntity = null;
    }

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[]
        {
            new GameActionParameterEntity.Description()
        };
    }
}
