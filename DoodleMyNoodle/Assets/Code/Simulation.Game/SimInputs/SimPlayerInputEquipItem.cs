using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class SimPlayerInputEquipItem : SimPlayerInput
{
    public fix2 ItemEntityPosition;
    public int ItemIndex;

    public SimPlayerInputEquipItem() { }

    public SimPlayerInputEquipItem(int ItemIndex, fix2 ItemEntityPosition)
    {
        this.ItemIndex = ItemIndex;
        this.ItemEntityPosition = ItemEntityPosition;
    }

    public override string ToString()
    {
        return $"SimPlayerInputEquipItem(player:{SimPlayerId.Value}, ItemPrefabID:{ItemIndex}, ItemEntityPosition:{ItemEntityPosition})";
    }
}

public class PawnControllerInputEquipItem : PawnControllerInputBase
{
    public fix2 ItemEntityPosition;
    public int ItemIndex;

    public PawnControllerInputEquipItem(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputEquipItem(Entity pawnController, int ItemIndex, fix2 ItemEntityPosition) : base(pawnController)
    {
        this.ItemIndex = ItemIndex;
        this.ItemEntityPosition = ItemEntityPosition;
    }
}