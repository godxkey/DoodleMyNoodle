using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnControllerInputUseItem : PawnControllerInputBase
{
    public int ItemIndex;
    public GameAction.UseData GameActionData;

    public PawnControllerInputUseItem(int itemIndex, GameAction.UseData gameActionData)
    {
        ItemIndex = itemIndex;
        GameActionData = gameActionData ?? throw new ArgumentNullException(nameof(gameActionData));
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, itemIndex: {ItemIndex}, gameActionData: {GameActionData})";
    }

}
