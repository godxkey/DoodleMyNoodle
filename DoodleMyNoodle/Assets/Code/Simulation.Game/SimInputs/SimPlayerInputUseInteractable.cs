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
        InteractablePosition = new int2() { x = Mathf.RoundToInt(position.x), y = Mathf.RoundToInt(position.y) };
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, interactablePosition:{InteractablePosition})";
    }
}