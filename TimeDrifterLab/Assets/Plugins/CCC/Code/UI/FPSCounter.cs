using UnityEngine;
using System.Collections;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] int frameSampleSize = 1;

    private float cummulatedFPS = 0;
    private int frameCounter = 0;

    void Update()
    {
        frameCounter++;

        if (frameCounter >= frameSampleSize)
        {
            PushToDisplay();
            frameCounter = 0;
            cummulatedFPS = 0;
        }

        cummulatedFPS += GetFPS();
    }

    void OnValidate()
    {
        frameSampleSize = Mathf.Max(1, frameSampleSize);
    }

    private void PushToDisplay()
    {
        textDisplay.text = (cummulatedFPS / frameCounter).ToString();
    }

    public static float GetFPS()
    {
        return 1f / Time.deltaTime;
    }

    public static float GetFixedFPS()
    {
        return 1f / Time.fixedDeltaTime;
    }
}
