using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[NetSerializable]
public class SimPlayerInputJump : SimPlayerInput
{
    public override string ToString()
    {
        return $"SimPlayerInputJump(player:{SimPlayerId.Value})";
    }
}