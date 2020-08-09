using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableOnContact : ObjectInteractable
{
    protected override void OnGamePresentationUpdate() 
    {
        int2 localPlayerTile = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn));
        int2 currentInteractableTile = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(SimEntity));

        if (((localPlayerTile.x == currentInteractableTile.x) && (localPlayerTile.y == currentInteractableTile.y)) && (CanTrigger()))
        {
            SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
            SimWorld.SubmitInput(inputUseInteractable);
        }
    }
}