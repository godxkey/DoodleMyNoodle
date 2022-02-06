using System;
using Unity.Entities;

public class PawnControllerInputUseItem : PawnControllerInputBase
{
    public int ItemIndex;
    public GameAction.UseParameters GameActionData;

    public PawnControllerInputUseItem(Entity pawnController, int itemIndex, GameAction.UseParameters gameActionData)
        : base(pawnController)
    {
        ItemIndex = itemIndex;
        GameActionData = gameActionData ?? throw new ArgumentNullException(nameof(gameActionData));
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, itemIndex: {ItemIndex}, gameActionData: {GameActionData})";
    }
}