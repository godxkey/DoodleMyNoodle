using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class SimPlayerInputEquipItem : SimPlayerInput
{
    public int2 ItemEntityPosition;
    public int ItemPrefabID;

    public SimPlayerInputEquipItem() { }

    public SimPlayerInputEquipItem(int ItemPrefabID, int2 ItemEntityPosition)
    {
        this.ItemPrefabID = ItemPrefabID;
        this.ItemEntityPosition = ItemEntityPosition;
    }

    public override string ToString()
    {
        return $"SimPlayerInputEquipItem(player:{SimPlayerId.Value}, ItemPrefabID:{ItemPrefabID}, ItemEntityPosition:{ItemEntityPosition})";
    }
}

public class PawnInputEquipItem : PawnControllerInputBase
{
    public int2 ItemEntityPosition;
    public int ItemPrefabID;

    public PawnInputEquipItem(Entity pawnController) : base(pawnController) { }

    public PawnInputEquipItem(Entity pawnController, int ItemPrefabID, int2 ItemEntityPosition) : base(pawnController)
    {
        this.ItemPrefabID = ItemPrefabID;
        this.ItemEntityPosition = ItemEntityPosition;
    }
}