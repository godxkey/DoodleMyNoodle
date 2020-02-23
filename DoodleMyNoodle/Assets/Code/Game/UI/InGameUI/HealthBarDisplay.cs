using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour
{
    public GameObject HealthpointsBar;
    public GameObject HealthDisplay;

    public float EndScale;
    public float EndXPosition;

    private void Start()
    {
        SimHealthStatComponent simHealthStatComponent = GetComponentInParent<SimHealthStatComponent>();
        ListenToHealthComponent(simHealthStatComponent);

        GetComponentInParent<SimComponentsLinker>()?.OnComponentAdded.AddListener(UpdateOnComponentAdded);
        GetComponentInParent<SimComponentsLinker>()?.OnComponentRemoved.AddListener(UpdateOnComponentRemoved);
    }

    public void UpdateOnComponentAdded(SimComponent newComponent)
    {
        if(newComponent is SimHealthStatComponent)
        {
            SimHealthStatComponent simHealthStatComponent = (SimHealthStatComponent)newComponent;
            ListenToHealthComponent(simHealthStatComponent);
        }
    }

    public void UpdateOnComponentRemoved(SimComponent oldComponent)
    {
        if (oldComponent is SimHealthStatComponent)
        {
            SimHealthStatComponent simHealthStatComponent = (SimHealthStatComponent)oldComponent;
            StopListenToHealthComponent(simHealthStatComponent);
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

    private void ListenToHealthComponent(SimHealthStatComponent simHealthStatComponent)
    {
        if (simHealthStatComponent != null)
        {
            simHealthStatComponent.OnStatChanged.AddListener(AjustDisplay);
            HealthDisplay.SetActive(true);
        }
    }

    private void StopListenToHealthComponent(SimHealthStatComponent simHealthStatComponent)
    {
        if (simHealthStatComponent != null)
        {
            simHealthStatComponent.OnStatChanged.RemoveListener(AjustDisplay);
            HealthDisplay.SetActive(false);
        }
    }
}
