using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class SimPlayerInputEquipItem : SimPlayerInput
{
    public Entity ChestEntity;
    public int ItemIndex;

    public SimPlayerInputEquipItem() { }

    public SimPlayerInputEquipItem(int ItemIndex, Entity chest)
    {
        this.ItemIndex = ItemIndex;
        this.ChestEntity = chest;
    }

    public override string ToString()
    {
        return $"SimPlayerInputEquipItem(player:{SimPlayerId.Value}, ItemPrefabID:{ItemIndex}, ItemEntityPosition:{ChestEntity})";
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