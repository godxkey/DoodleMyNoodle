using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputUseInteractable : SimPlayerInput
{
    public Vector3Int InteractablePosition;

    public SimPlayerInputUseInteractable() { }

    public SimPlayerInputUseInteractable(Vector3 position)
    {
        InteractablePosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, interactablePosition:{InteractablePosition})";
    }
}