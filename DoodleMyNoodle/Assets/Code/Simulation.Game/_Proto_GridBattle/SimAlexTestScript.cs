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
                GetComponent<SimInventoryComponent>()?.TakeItem(ItemBank.Instance.GetItemWithSameName("Test"));
            }

            if (keyCodeInput.keyCode == KeyCode.M && keyCodeInput.state == SimInputKeycode.State.Pressed)
            {
                GetComponent<SimInventoryComponent>()?.DropItem(ItemBank.Instance.GetItemWithSameName("Test"));
            }
        }
    }
}
