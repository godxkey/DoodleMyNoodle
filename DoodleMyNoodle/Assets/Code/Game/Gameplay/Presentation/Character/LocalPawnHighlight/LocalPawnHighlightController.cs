using UnityEngine;
using Unity.Entities;

public class LocalPawnHighlightController : GamePresentationSystem<LocalPawnHighlightController>
{
    [SerializeField] private Color _newTurnHighlightColor = Color.white;

    public override void PresentationPostSimulationTick()
    {
        base.PresentationPostSimulationTick();

        // On player's new turn
        if (Cache.DEPRECATED_IsNewTurn && Cache.DEPRECATED_CanLocalPlayerPlay) 
        {
            // Find local pawn's doodle
            if (Cache.LocalPawn != Entity.Null &&
                BindedSimEntityManaged.InstancesMap.TryGetValue(Cache.LocalPawn, out GameObject localPawnViewGO))
            {
                if (localPawnViewGO.TryGetComponent(out DoodleDisplay doodleDisplay))
                {
                    var highlightParams = HighlightService.Params.Default;
                    highlightParams.Color = _newTurnHighlightColor;
                    highlightParams.FlickerSpeed = HighlightService.FlickerSpeed.Fast;
                    highlightParams.Intensity = HighlightService.Intensity.High;

                    HighlightService.HighlightSprite(doodleDisplay.SpriteRenderer, highlightParams);
                }
            }
        }
    }
}
