using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : GamePresentationBehaviour
{
    [SerializeField] private GameObject _outline;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponentData(GetInteractableEntity(), out Interacted interacted) && interacted.Value && CanTrigger())
        {
            _outline.SetActive(false);

            InteractionTriggeredByInput();

            Debug.Log("Interactable Triggered");
        }
    }

    protected virtual void InteractionTriggeredByInput() { }

    private void OnMouseOver()
    {
        if (CanTrigger())
        {
            _outline.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (CanTrigger())
        {
            _outline.SetActive(false);
        }
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

            Debug.Log("Triggering Interactable");
        }
    }

    private bool CanTrigger()
    {
        Entity interactable = GetInteractableEntity();
        Interactable interactableData = SimWorld.GetComponentData<Interactable>(interactable);

        if(SimWorld.TryGetComponentData(interactable, out Interacted interactedData))
        {
            return interactableData.OnlyOnce ? !interactedData.Value : true;
        }
        else
        {
            return true;
        }
    }

    private Entity GetInteractableEntity()
    {
        Entity tile = CommonReads.GetTileEntity(SimWorld, new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        return CommonReads.GetFirstTileAddonWithComponent<Interactable>(SimWorld, tile);
    }
}