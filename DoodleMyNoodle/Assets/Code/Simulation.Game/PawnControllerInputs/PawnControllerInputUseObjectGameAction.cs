using Unity.Entities;
using Unity.Mathematics;

public class PawnControllerInputUseObjectGameAction : PawnControllerInputBase
{
    public fix2 ObjectPosition;
    public Action.UseParameters GameActionData;

    public PawnControllerInputUseObjectGameAction(Entity pawnController, fix2 objectPosition, Action.UseParameters gameActionData) : base(pawnController)
    {
        ObjectPosition = objectPosition;
        GameActionData = gameActionData;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, objectPosition: {ObjectPosition})";
    }
}