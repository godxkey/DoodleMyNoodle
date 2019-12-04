using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimAlexTestScript : SimComponent, ISimInputProcessor
{
    public void ProcessInput(SimInput input)
    {
        if (input is SimInputKeycode keyCodeInput && keyCodeInput.keyCode == KeyCode.M && keyCodeInput.state == SimInputKeycode.State.Pressed) 
        {
            GetComponent<SimHealthStatComponent>()?.DecreaseValue(1);
        }
    }
}
