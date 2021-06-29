using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputMovingCharacter : SimPlayerInput
{
    public fix2 Direction;

    public SimPlayerInputMovingCharacter() { }

    public SimPlayerInputMovingCharacter(fix2 Direction) 
    {
        this.Direction = Direction;
    }

    public override string ToString()
    {
        return $"SimPlayerMovingCharacter(player:{SimPlayerId.Value})";
    }
}