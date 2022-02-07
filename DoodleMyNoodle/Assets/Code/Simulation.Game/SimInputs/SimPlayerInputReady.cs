using Unity.Entities;

[NetSerializable]
public class SimPlayerInputReady : SimPlayerInput
{
    public bool IsReady = false;

    public SimPlayerInputReady() { }

    public SimPlayerInputReady(bool isReady) 
    {
        IsReady = isReady;
    }
}

public class PawnControllerInputNextTurn : PawnControllerInputBase
{
    public bool IsReady = false;

    public PawnControllerInputNextTurn(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputNextTurn(Entity pawnController, bool isReady) : base(pawnController)
    {
        IsReady = isReady;
    }
}