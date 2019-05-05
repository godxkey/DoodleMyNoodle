using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimInput
{
    public SimPlayerId player;

    public SimInput(SimPlayerId player)
    {

    }


    public abstract void Execute(SimWorld world);
}
