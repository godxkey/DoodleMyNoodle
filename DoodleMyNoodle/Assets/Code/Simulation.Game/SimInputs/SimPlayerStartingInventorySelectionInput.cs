using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerStartingInventorySelectionInput : SimPlayerInput
{
    public int KitNumber;

    public SimPlayerStartingInventorySelectionInput() : base() { }

    public SimPlayerStartingInventorySelectionInput(int KitNumber) : base()
    {
        this.KitNumber = KitNumber;
    }
}

public class PawnControllerInputStartingInventorySelection : PawnControllerInputBase
{
    public int KitNumber;

    public PawnControllerInputStartingInventorySelection(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputStartingInventorySelection(Entity pawnController, int KitNumber) : base(pawnController)
    {
        this.KitNumber = KitNumber;
    }
}