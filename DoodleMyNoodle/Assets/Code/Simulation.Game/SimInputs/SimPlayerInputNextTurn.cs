using Unity.Entities;

[NetSerializable]
public class SimPlayerInputNextTurn : SimPlayerInput
{
    public bool ReadyForNextTurn = false;

    public SimPlayerInputNextTurn() { }

    public SimPlayerInputNextTurn(bool isReady) 
    {
        ReadyForNextTurn = isReady;
    }
}

public class PawnControllerInputNextTurn : PawnControllerInputBase
{
    public bool ReadyForNextTurn = false;

    public PawnControllerInputNextTurn(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputNextTurn(Entity pawnController, bool isReady) : base(pawnController)
    {
        ReadyForNextTurn = isReady;
    }
}