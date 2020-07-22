using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : BindedPresentationEntityComponent
{
    [SerializeField] private GameObject _outline;

    private bool _previousInteractedState = false;

    protected override void OnGamePresentationUpdate() 
    {
        base.OnGamePresentationUpdate();
    }

    public override void OnPostSimulationTick()
    {
        Entity InteractableEntity = SimEntity;
        if (InteractableEntity != Entity.Null)
        {
            if (SimWorld.TryGetComponentData(InteractableEntity, out Interacted interactedData))
            {
                if (interactedData.Value != _previousInteractedState)
                {
                    if (interactedData.Value)
                    {
                        _outline.SetActive(false);

                        InteractionTriggeredByInput();
                    }
                    else
                    {
                        InteractionReset();
                    }

                    _previousInteractedState = interactedData.Value;
                }
            }
        }
    }

    protected virtual void InteractionTriggeredByInput() { }

    protected virtual void InteractionReset() { }

    private void OnMouseOver()
    {
        if (CanTrigger())
        {
            _outline.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        _outline.SetActive(false);
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

    private bool CanTrigger()
    {
        Entity interactable = SimEntity;
        if(interactable == Entity.Null)
        {
            return false;
        }

        if(SimWorld.TryGetComponentData(interactable, out Interactable interactableData))
        {
            if (SimWorld.TryGetComponentData(interactable, out Interacted interactedData))
            {
                return interactableData.OnlyOnce ? !interactedData.Value : true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}