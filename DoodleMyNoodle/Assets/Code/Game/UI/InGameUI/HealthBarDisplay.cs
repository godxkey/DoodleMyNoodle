using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour
{
    public GameObject HealthpointsBar;
    public GameObject HealthDisplay;

    private void Start()
    {
        SimHealthStatComponent simHealthStatComponent = GetComponentInParent<SimHealthStatComponent>();
        ListenToHealthComponent(simHealthStatComponent);

        GetComponentInParent<SimComponentsLinker>()?.OnComponentAdded.AddListener(UpdateOnComponentAdded);
        GetComponentInParent<SimComponentsLinker>()?.OnComponentRemoved.AddListener(UpdateOnComponentRemoved);
    }

    public void UpdateOnComponentAdded(SimComponent newComponent)
    {
        if (newComponent is SimHealthStatComponent)
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

        HealthpointsBar.transform.localScale = new Vector3(Mathf.Lerp(0, 1, healthpointsRatio), 1, 1);
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
