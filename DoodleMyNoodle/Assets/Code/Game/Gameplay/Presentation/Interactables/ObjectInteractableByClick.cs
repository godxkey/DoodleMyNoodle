using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : GamePresentationBehaviour
{
    [SerializeField] private HighlightContourDisplay _highlightContour;

    [SerializeField] private bool _canTriggerMultipleTime = false;

    private bool _hasBeenInteractedWith = false;

    protected override void OnGamePresentationUpdate()
    {
        Entity tile = CommonReads.GetTileEntity(SimWorld, new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        Entity interactableEntity = CommonReads.GetSingleTileAddonOfType<InteractableObjectTag>(SimWorld, tile);

        if (SimWorld.TryGetComponentData(interactableEntity, out Interacted interacted) && interacted.Value && CanTrigger())
        {
            // Interactable has been triggered
            _hasBeenInteractedWith = true;

            _highlightContour.ChangeVisibility(false);

            InteractionTriggeredByInput();

            Debug.Log("Interactable Triggered");
        }
    }

    protected virtual void InteractionTriggeredByInput() { }

    private void OnMouseOver()
    {
        if (!CanTrigger())
            return;

        _highlightContour.ChangeVisibility(true);
    }

    private void OnMouseExit()
    {
        if (!CanTrigger())
            return;

        _highlightContour.ChangeVisibility(false);
    }

    private void OnMouseDown()
    {
        if (!CanTrigger())
            return;

        // TODO : One day once it's fix send directly the entity
        // BindedSimEntityManaged bindingEntityComponent = GetComponent<BindedSimEntityManaged>();
        // bindingEntityComponent.SimEntity

        SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
        SimWorld.SubmitInput(inputUseInteractable);

        Debug.Log("Triggering Interactable");
    }

    private bool CanTrigger()
    {
        return _canTriggerMultipleTime ? true : !_hasBeenInteractedWith;
    }
}