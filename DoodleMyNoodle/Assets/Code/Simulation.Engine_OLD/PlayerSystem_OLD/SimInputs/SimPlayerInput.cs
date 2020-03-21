using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInput : SimInput
{
    // this will be assigned by the server when its about to enter the simulation
    public SimPlayerId SimPlayerIdOld;
    public PersistentId SimPlayerId;
}
