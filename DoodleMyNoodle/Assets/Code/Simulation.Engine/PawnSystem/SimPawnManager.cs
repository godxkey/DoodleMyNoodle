using System;
using System.Collections.Generic;

public class SimPawnManager : SimEventSingleton<SimPawnManager>,
    ISimEventListener<SimPlayerCreatedEventData>,
    ISimEventListener<SimPlayerDestroyedEventData>
{
    public override void OnSimStart()
    {
        base.OnSimStart();

        SimGlobalEventEmitter.RegisterListener<SimPlayerCreatedEventData>(this);
        SimGlobalEventEmitter.RegisterListener<SimPlayerDestroyedEventData>(this);
    }

    public void OnEventRaised(in SimPlayerCreatedEventData eventData)
    {
        // if the player can control pawns
        if (eventData.PlayerEntity.GetComponent(out SimTargetPawnComponent pawnController))
        {
            // and the player has no pawn
            if (pawnController.Target == null)
            {
                // find uncontrolled pawn
                SimPawnInterfaceComponent uncontrolledPawn = SimPawnHelpers.FindUncontrolledPawn();

                if (uncontrolledPawn)
                {
                    // hook them together
                    HookControllerWithPawn(pawnController, uncontrolledPawn);
                }
            }
        }
    }

    public void OnEventRaised(in SimPlayerDestroyedEventData eventData)
    {
        if(eventData.PlayerEntity.GetComponent(out SimTargetPawnComponent targetPawnComponent))
        {
            UnhookControllerFromPawn(targetPawnComponent);
        }
    }

    public void HookControllerWithPawn(SimTargetPawnComponent controller, SimPawnInterfaceComponent pawn)
    {
        if(controller.Target)
        {
            DebugService.LogError($"Cannot hook pawn '{pawn.gameObject.name}' with controller '{controller.gameObject}'." +
                (controller.Target ? $" The controller is already controlling '{controller.Target.gameObject.name}." : $"") +
                $" Please unhook before hooking again.");
            return;
        }

        // change pawn
        controller.Target = pawn;
        
        // notify
        NotifyPawnControllerOfPawnChange(controller.SimEntity);
    }

    public void UnhookControllerFromPawn(SimTargetPawnComponent controller)
    {
        controller.Target = null;

        if (controller.DestroySelfIfNoTarget)
        {
            // destroy controller
            Simulation.Destroy(controller);
        }
        else
        {
            // notify controller
            NotifyPawnControllerOfPawnChange(controller.SimEntity);
        }
    }

    List<ISimTargetPawnChangeListener> _cachedISimTargetPawnChangeListenerComponents;

    void NotifyPawnControllerOfPawnChange(SimEntity controller)
    {
        controller.GetComponents<ISimTargetPawnChangeListener>(_cachedISimTargetPawnChangeListenerComponents);
        foreach (ISimTargetPawnChangeListener pawnChangeObserver in _cachedISimTargetPawnChangeListenerComponents)
        {
            pawnChangeObserver.OnTargetPawnChanged();
        }
    }
}