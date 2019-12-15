using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimAlexTestScript : SimComponent, ISimInputProcessor
{
    public void ProcessInput(SimInput input)
    {
        if (input is SimInputKeycode keyCodeInput)
        {
            if (keyCodeInput.keyCode == KeyCode.N && keyCodeInput.state == SimInputKeycode.State.Pressed)
            {
                GetComponent<SimInventoryContainer>()?.TakeItem(ItemBank.Instance.GetItem("Test"));
            }

            if (keyCodeInput.keyCode == KeyCode.M && keyCodeInput.state == SimInputKeycode.State.Pressed)
            {
                GetComponent<SimInventoryContainer>()?.DropItem(ItemBank.Instance.GetItem("Test"));
            }
        }
    }
}
