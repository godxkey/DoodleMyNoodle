using System;
using UnityEngine;
using UnityEngineX;

public class DisplayWhileSignalActive : BindedPresentationEntityComponent
{
    public GameObject ContainerToDisplay;

    protected override void OnGamePresentationUpdate()
    {
        if(SimWorld.TryGetComponent(SimEntity, out Signal signal)) 
        {
            ContainerToDisplay.SetActive(signal.Value);
        }
    }
}