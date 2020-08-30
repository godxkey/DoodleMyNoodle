using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;
using Unity.Entities;

public class EntityDamagedFlash : BindedPresentationEntityComponent
{
    [SerializeField] private SpriteRenderer entityRenderer;

    public override void OnPostSimulationTick()
    {
        if (SimEntity != Entity.Null && SimWorld.HasComponent<Damaged>(SimEntity))
        {
            StartFlash();
        }
    }

    protected override void OnGamePresentationUpdate() { }

    private void StartFlash()
    {
        HighlightService.Params highlightParams = HighlightService.Params.Default;
        highlightParams.Color = Color.red;
        HighlightService.HighlightSprite(entityRenderer, highlightParams);
    }
}