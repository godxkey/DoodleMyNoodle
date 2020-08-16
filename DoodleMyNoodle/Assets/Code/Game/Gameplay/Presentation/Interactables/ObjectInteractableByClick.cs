using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : InteractableEntityView
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _highlighted = false;

    protected override void OnGamePresentationUpdate() { }

    private void OnMouseOver()
    {
        if (CanTrigger())
        {
            SetHighlighted(true);
        }
    }

    private void OnMouseExit()
    {
        SetHighlighted(false);
    }

    private void OnMouseDown()
    {
        if (CanTrigger())
        {
            // TODO : One day once it's fix send directly the entity
            // BindedSimEntityManaged bindingEntityComponent = GetComponent<BindedSimEntityManaged>();
            // bindingEntityComponent.SimEntity

            SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
            SimWorld.SubmitInput(inputUseInteractable);
        }
    }

    private void SetHighlighted(bool highlighted)
    {
        if (_highlighted == highlighted)
            return;

        _highlighted = highlighted;

        if (highlighted)
        {
            HighlightService.Params args = HighlightService.Params.Default;

            args.Duration = HighlightService.Duration.Long;
            args.FlickerSpeed = HighlightService.FlickerSpeed.Slow;
            args.Intensity = HighlightService.Intensity.Normal;
            args.AnimStart = HighlightService.AnimStart.MidHighlight;

            HighlightService.HighlightSprite(_spriteRenderer, args);
        }
        else
        {
            HighlightService.StopHighlight(_spriteRenderer);
        }
    }
}