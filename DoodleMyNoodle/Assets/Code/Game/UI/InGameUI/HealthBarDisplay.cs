using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour
{
    public GameObject HealthpointsBar;

    public float EndScale;
    public float EndXPosition;

    private void Start()
    {
        SimHealthStatComponent simHealthStatComponent = GetComponentInParent<SimHealthStatComponent>();
        if (simHealthStatComponent != null)
        {
            simHealthStatComponent.OnStatChanged.AddListener(AjustDisplay);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AjustDisplay(float HealthCurrentValue, float PreviousValue, float HealthMaxValue)
    {
        float healthpointsRatio = HealthCurrentValue / HealthMaxValue;
        Vector3 pos = HealthpointsBar.transform.localPosition;
        HealthpointsBar.transform.localPosition = new Vector3(Mathf.Lerp(EndXPosition, pos.x, healthpointsRatio), pos.y, pos.z);

        Vector3 scale = HealthpointsBar.transform.localScale;
        HealthpointsBar.transform.localScale = new Vector3(Mathf.Lerp(EndScale, scale.x, healthpointsRatio), scale.y, scale.z);
    }
}
