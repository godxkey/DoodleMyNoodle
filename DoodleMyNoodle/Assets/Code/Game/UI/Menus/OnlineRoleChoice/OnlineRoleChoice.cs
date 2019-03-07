using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineRoleChoice : MonoBehaviour
{
    public void LaunchClient()
    {
        WaitSpinner.Enable(this);

        this.DelayedCall(3, LaunchServer);
    }

    public void LaunchServer()
    {
        WaitSpinner.Disable(this);
    }
}
