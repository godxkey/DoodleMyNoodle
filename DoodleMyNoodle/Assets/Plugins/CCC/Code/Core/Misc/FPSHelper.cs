using UnityEngine;

public static class FPSHelper
{
    static public float CurrentFPS
    {
        get
        {
            return 1f / Time.deltaTime;
        }
    }

    static public float CurrentFixedFPS
    {
        get
        {
            return 1f / Time.fixedDeltaTime;
        }
    }
}
