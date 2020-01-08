using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity with this can be controlled
/// </summary>
public class SimPawnComponent : SimComponent
{
    // the AI controller to instantiate if we want the pawn to be controlled by an AI
    //      e.g: the 'green-archer' pawn might reference a 'archer AI controller' prefab
    [SerializeField, Tooltip("Pawn controller to spawn OnAwake. Leave empty if you want the pawn to be 'brain-less'.")]
    private SimPawnControllerComponent _controllerPrefab;

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        // if we have no current controller, try falling back to our 'defaultAIController'
        InstantiateAIControllerAndHook();
    }

    public void InstantiateAIControllerAndHook()
    {
        if (_controllerPrefab)
        {
            // instantiate
            SimPawnControllerComponent newController = Simulation.Instantiate(_controllerPrefab);

            // hook
            SimPawnManager.Instance.HookControllerWithPawn(newController, GetComponent<SimPawnComponent>());
        }
    }
}