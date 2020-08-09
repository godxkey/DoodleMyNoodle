using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerInputSelectStartingInventory : SimPlayerInput
{
    public int KitNumber;

    public SimPlayerInputSelectStartingInventory() : base() { }

    public SimPlayerInputSelectStartingInventory(int KitNumber) : base()
    {
        this.KitNumber = KitNumber;
    }
}

public class PawnControllerInputSelectStartingInventory : PawnControllerInputBase
{
    public int KitNumber;

    public PawnControllerInputSelectStartingInventory(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputSelectStartingInventory(Entity pawnController, int KitNumber) : base(pawnController)
    {
        this.KitNumber = KitNumber;
    }
}