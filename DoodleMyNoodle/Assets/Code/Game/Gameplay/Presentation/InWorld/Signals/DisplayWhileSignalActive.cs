using System;
using UnityEngine;
using UnityEngineX;

public class DisplayWhileSignalActive : BindedPresentationEntityComponent
{
    public GameObject ContainerToDisplay;

    public override void PresentationUpdate()
    {
        if(SimWorld.TryGetComponent(SimEntity, out Signal signal)) 
        {
            ContainerToDisplay.SetActive(signal.Value);
        }
    }
}