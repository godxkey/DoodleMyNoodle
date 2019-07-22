using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompBallRandomMove : SimComponent, ISimInputHandler
{
    public bool HandleInput(SimInput input)
    {
        if(input is SimInputKeycode inputKeycode && inputKeycode.keyCode == KeyCode.Space)
        {
            FixVector3 dir = Simulation.Random.Direction3D();
            simTransform.localPosition += dir;

            return true;
        }

        return false;
    }
}
