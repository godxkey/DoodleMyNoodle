using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimTestScript : SimComponent, ISimInputProcessor
{
    public void ProcessInput(SimInput input)
    {
        if (input is SimInputKeycode keyCodeInput && keyCodeInput.keyCode == KeyCode.M && keyCodeInput.state == SimInputKeycode.State.Pressed) 
        {
            GetComponent<SimPlayerStatsComponent>()?.ChangeValue(-1);
        }
    }
}
