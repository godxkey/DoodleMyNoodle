using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineRoleChoice : MonoBehaviour
{
    public void LaunchClient()
    {
        WaitSpinnerService.Enable(this);

        this.DelayedCall(3, LaunchServer);
    }

    public void LaunchServer()
    {
        WaitSpinnerService.Disable(this);
    }
}
