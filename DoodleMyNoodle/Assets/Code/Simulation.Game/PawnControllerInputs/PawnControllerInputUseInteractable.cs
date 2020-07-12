using System;
using Unity.Entities;
using UnityEngine;

public class PawnControllerInputUseInteractable : PawnControllerInputBase
{
    public Vector3Int InteractablePosition;

    public PawnControllerInputUseInteractable(Entity pawnController, Vector3Int interactablePosition) : base(pawnController)
    {
        InteractablePosition = interactablePosition;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, interactablePosition: {InteractablePosition})";
    }
}