using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInputInterface : GameMonoBehaviour
{
    public override void OnGameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SimulationController.instance.SubmitInput(new SimInputLog() { message = "hello!" });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SimulationController.instance.SubmitInput(new SimInputInstantiate() { blueprintId = new SimBlueprintId(1) });
        }
    }
}
