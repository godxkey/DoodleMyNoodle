using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputMove : SimPlayerInput
{
    public fix2 NewDirection;

    public SimPlayerInputMove() { }

    public SimPlayerInputMove(fix2 newDirection) 
    {
        this.NewDirection = newDirection;
    }

    public override string ToString()
    {
        return $"SimPlayerInputMove(player:{SimPlayerId.Value}, newDirection:{NewDirection})";
    }
}