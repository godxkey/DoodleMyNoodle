using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableByClick : ObjectInteractable
{
    [SerializeField] private GameObject _outline;
    
    protected override void OnGamePresentationUpdate() { }

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
}