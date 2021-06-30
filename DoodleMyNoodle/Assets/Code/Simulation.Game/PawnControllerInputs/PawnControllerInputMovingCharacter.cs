using System;
using Unity.Entities;

public class PawnControllerInputMovingCharacter : PawnControllerInputBase
{
    public fix2 Direction;

    public PawnControllerInputMovingCharacter(Entity pawnController, fix2 Direction) : base(pawnController)
    {
        this.Direction = Direction;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, direction: {Direction})";
    }
}