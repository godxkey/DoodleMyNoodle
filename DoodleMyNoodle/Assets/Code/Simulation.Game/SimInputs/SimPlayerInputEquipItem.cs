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
    public Entity ChestEntity;
    public int ItemIndex;

    public PawnControllerInputEquipItem(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputEquipItem(Entity pawnController, int itemIndex, Entity chestEntity) : base(pawnController)
    {
        ItemIndex = itemIndex;
        ChestEntity = chestEntity;
    }
}