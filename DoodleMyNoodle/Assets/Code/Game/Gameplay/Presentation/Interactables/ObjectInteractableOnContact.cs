using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class ObjectInteractableOnContact : InteractableEntityView
{
    protected override void OnGamePresentationUpdate() 
    {
        if(SimWorldCache.LocalPawn != Entity.Null)
        {
            int2 localPlayerTile = SimWorldCache.LocalPawnTile;
            int2 currentInteractableTile = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(SimEntity));

            if ((localPlayerTile.x == currentInteractableTile.x) && (localPlayerTile.y == currentInteractableTile.y) && CanTrigger())
            {
                SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
                SimWorld.SubmitInput(inputUseInteractable);
            }
        }
    }
}