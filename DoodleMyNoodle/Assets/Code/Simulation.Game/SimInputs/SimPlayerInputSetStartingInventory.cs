using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerInputSetStartingInventory : SimPlayerInput
{
    public int KitNumber;

    public SimPlayerInputSetStartingInventory() : base() { }

    public SimPlayerInputSetStartingInventory(int KitNumber) : base()
    {
        this.KitNumber = KitNumber;
    }
}

public class PawnControllerInputSetStartingInventory : PawnControllerInputBase
{
    public int KitNumber;

    public PawnControllerInputSetStartingInventory(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputSetStartingInventory(Entity pawnController, int KitNumber) : base(pawnController)
    {
        this.KitNumber = KitNumber;
    }
}