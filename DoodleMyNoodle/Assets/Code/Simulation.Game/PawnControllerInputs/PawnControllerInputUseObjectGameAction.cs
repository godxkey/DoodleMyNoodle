using Unity.Entities;
using Unity.Mathematics;

public class PawnControllerInputUseObjectGameAction : PawnControllerInputBase
{
    public fix2 ObjectPosition;
    public GameAction.UseParameters GameActionData;

    public PawnControllerInputUseObjectGameAction(Entity pawnController, fix2 objectPosition, GameAction.UseParameters gameActionData) : base(pawnController)
    {
        ObjectPosition = objectPosition;
        GameActionData = gameActionData;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, objectPosition: {ObjectPosition})";
    }
}