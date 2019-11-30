using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour
{
    public GameObject HealthpointsBar;

    public float EndScale;
    public float EndXPosition;

    public void AjustDisplay(float HealthCurrentValue, float HealthMaxValue)
    {
        float HealthpointsRatio = HealthCurrentValue / HealthMaxValue;
        Vector3 Pos = HealthpointsBar.transform.localPosition;
        HealthpointsBar.transform.localPosition = new Vector3(Mathf.Lerp(EndXPosition, Pos.x, HealthpointsRatio), Pos.y, Pos.z);

        Vector3 Scale = HealthpointsBar.transform.localScale;
        HealthpointsBar.transform.localScale = new Vector3(Mathf.Lerp(EndScale, Scale.x, HealthpointsRatio), Scale.y, Scale.z);
    }
}
