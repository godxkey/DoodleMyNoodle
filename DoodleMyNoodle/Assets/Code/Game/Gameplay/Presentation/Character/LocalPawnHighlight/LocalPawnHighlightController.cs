using UnityEngine;
using Unity.Entities;

public class LocalPawnHighlightController : GamePresentationBehaviour
{
    public override void OnPostSimulationTick()
    {
        base.OnPostSimulationTick();

        if (SimWorld.HasSingleton<NewTurnEventData>()) // On new turn
        {
            // Find local pawn's doodle
            if (SimWorldCache.LocalPawn != Entity.Null &&
                BindedSimEntityManaged.InstancesMap.TryGetValue(SimWorldCache.LocalPawn, out GameObject localPawnViewGO))
            {
                if (localPawnViewGO.TryGetComponent(out DoodleDisplay doodleDisplay))
                {
                    HighlightService.HighlightSprite(doodleDisplay.SpriteRenderer, HighlightService.Duration.UntilManuallyStopped);
                }
            }
        }
    }

    protected override void OnGamePresentationUpdate() { }
}
