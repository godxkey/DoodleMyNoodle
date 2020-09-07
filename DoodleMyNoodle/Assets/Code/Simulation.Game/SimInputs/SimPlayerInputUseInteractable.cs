using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputUseInteractable : SimPlayerInput
{
    public int2 InteractablePosition;

    public SimPlayerInputUseInteractable() { }

    public SimPlayerInputUseInteractable(Vector3 position)
    {
        InteractablePosition = Helpers.GetTile(position);
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, interactablePosition:{InteractablePosition})";
    }
}