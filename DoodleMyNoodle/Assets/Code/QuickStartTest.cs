using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStartTest : MonoBehaviour
{
    static bool done = false;

    void Start()
    {
        if (done == false)
        {
            done = true;
            QuickStart.Start(QuickStartAssets.instance.defaultSettings);
        }
    }

}
