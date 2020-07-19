using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : GamePresentationBehaviour
{
    [SerializeField] private GameObject _outline;

    private bool _previousInteractedState = false;

    protected override void OnGamePresentationUpdate() { }

    public override void OnPostSimulationTick()
    {
        if (SimWorld.TryGetComponentData(GetInteractableEntity(), out Interacted interactedData))
        {
            if (interactedData.Value != _previousInteractedState)
            {
                if (interactedData.Value)
                {
                    _outline.SetActive(false);

                    InteractionTriggeredByInput();

                    Debug.Log("Interactable Triggered");
                }
                else
                {
                    InteractionReset();

                    Debug.Log("Interactable Reset");
                }

                _previousInteractedState = interactedData.Value;
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

            Debug.Log("Triggering Interactable");
        }
    }

    private bool CanTrigger()
    {
        Entity interactable = GetInteractableEntity();
        if(interactable == Entity.Null)
        {
            return false;
        }

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