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

public class PawnInputNextTurn : PawnControllerInputBase
{
    public bool ReadyForNextTurn = false;

    public PawnInputNextTurn(Entity pawnController) : base(pawnController) { }

    public PawnInputNextTurn(Entity pawnController, bool isReady) : base(pawnController)
    {
        ReadyForNextTurn = isReady;
    }
}