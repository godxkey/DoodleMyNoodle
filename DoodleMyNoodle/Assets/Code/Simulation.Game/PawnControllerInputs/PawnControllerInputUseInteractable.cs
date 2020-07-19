using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PawnControllerInputUseInteractable : PawnControllerInputBase
{
    public int2 InteractablePosition;

    public PawnControllerInputUseInteractable(Entity pawnController, int2 interactablePosition) : base(pawnController)
    {
        InteractablePosition = interactablePosition;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, interactablePosition: {InteractablePosition})";
    }
}