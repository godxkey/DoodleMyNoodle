using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using CCC.Fix2D;
using UnityEngineX;

public class ObjectInteractableOnContact : InteractableEntityView
{
    protected override void OnGamePresentationUpdate() 
    {
        if(Cache.LocalPawn != Entity.Null)
        {
            int2 localPlayerTile = Cache.LocalPawnTile;
            int2 currentInteractableTile = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(SimEntity));

            if ((localPlayerTile.x == currentInteractableTile.x) && (localPlayerTile.y == currentInteractableTile.y) && CanTrigger())
            {
                SimPlayerInputUseInteractable inputUseInteractable = new SimPlayerInputUseInteractable(transform.position);
                SimWorld.SubmitInput(inputUseInteractable);
            }
        }
    }
}