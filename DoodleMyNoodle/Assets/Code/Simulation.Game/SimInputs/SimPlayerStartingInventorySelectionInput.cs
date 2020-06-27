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

public class PawnStartingInventorySelectionInput : PawnControllerInputBase
{
    public int KitNumber;

    public PawnStartingInventorySelectionInput(Entity pawnController) : base(pawnController) { }

    public PawnStartingInventorySelectionInput(Entity pawnController, int KitNumber) : base(pawnController)
    {
        this.KitNumber = KitNumber;
    }
}