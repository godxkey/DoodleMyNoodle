using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitySelectionSurveyController : SurveyBaseController
{
    private Entity _selectedEntity;

    private EntitySelectionDisplay _previouslySelectedEntityDisplay;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        if (_selectedEntity != Entity.Null)
        {
            results.Add(new GameActionParameterEntity.Data(Cache.SimWorld.GetComponentData<FixTranslation>(_selectedEntity)));
        }
        return results;
    }

    protected override string GetDebugResult()
    {
        return "Entity Found";
    }

    protected override void OnUpdate()
    {
        Vector3 mousePos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            EntitySelectionDisplay _entitySelection = hit.collider.gameObject.GetComponent<EntitySelectionDisplay>();
            if (_entitySelection != null)
            {
                if (VerifyEntityForParameters(_entitySelection.SimEntity))
                {
                    if (_entitySelection != _previouslySelectedEntityDisplay)
                    {
                        if (_previouslySelectedEntityDisplay)
                        {
                            _previouslySelectedEntityDisplay.StopOveringOnDisplay();
                        }
                        _entitySelection.StartOveringOnDisplay();
                        _previouslySelectedEntityDisplay = _entitySelection;
                    }
                }
                else
                {
                    if (_previouslySelectedEntityDisplay)
                    {
                        _previouslySelectedEntityDisplay.StopOveringOnDisplay();
                    }

                    _previouslySelectedEntityDisplay = null;
                }
                
            }
        }

        if (Input.GetMouseButtonDown(0) && _previouslySelectedEntityDisplay != null)
        {
            _selectedEntity = _previouslySelectedEntityDisplay.SimEntity;
            _previouslySelectedEntityDisplay.StopOveringOnDisplay();
            Complete();
        }
    }

    protected override IEnumerator SurveyLoop()
    {
        yield return null;
    }

    private bool VerifyEntityForParameters(Entity entity)
    {
        if (entity != null && _parameters.Length > 0)
        {
            if (_parameters[0] is GameActionParameterEntity.Description ParameterEntity)
            {
                if (!ParameterEntity.IncludeSelf && entity == Cache.LocalPawn)
                {
                    return false;
                }

                if (ParameterEntity.RequiresAttackableEntity)
                {
                    if (!Cache.SimWorld.HasComponent<Health>(entity))
                    {
                        return false;
                    }
                }

                if(Cache.SimWorld.TryGetComponentData(entity, out FixTranslation entityTranslation))
                {
                    fix squaredDistance = fix3.DistanceSquared(Cache.LocalPawnPosition, entityTranslation.Value);
                    if (squaredDistance > ((fix)Mathf.Pow(ParameterEntity.RangeFromInstigator, 2)))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }
}
