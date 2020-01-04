using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this to a pawn so that it spawns a controller for itself
/// </summary>
[RequireComponent(typeof(SimPawnInterfaceComponent))]
public class SimAIControllerToSpawnComponent : SimComponent
{
    // the AI controller to instantiate if we want the pawn to be controlled by an AI
    //      e.g: the 'green-archer' pawn might reference a 'archer AI controller' prefab
    [SerializeField]
    private SimTargetPawnComponent _controllerPrefab;

    public void InstantiateAIControllerAndHook()
    {
        if (_controllerPrefab)
        {
            // instantiate
            SimTargetPawnComponent newController = Simulation.Instantiate(_controllerPrefab);

            // hook
            SimPawnManager.Instance.HookControllerWithPawn(newController, GetComponent<SimPawnInterfaceComponent>());
        }
    }

    public override void OnSimStart()
    {
        base.OnSimStart();

        // if we have no current controller, try falling back to our 'defaultAIController'
        InstantiateAIControllerAndHook();
    }
}
