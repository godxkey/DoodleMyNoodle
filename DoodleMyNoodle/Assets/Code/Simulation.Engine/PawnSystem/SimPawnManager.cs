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
        if (eventData.PlayerEntity.GetComponent(out SimPawnControllerComponent pawnController))
        {
            // and the player has no pawn
            if (pawnController.TargetPawn == null)
            {
                // find uncontrolled pawn
                SimPawnComponent uncontrolledPawn = SimPawnHelpers.FindUncontrolledPawn();

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
        if(eventData.PlayerEntity.GetComponent(out SimPawnControllerComponent targetPawnComponent))
        {
            UnhookControllerFromPawn(targetPawnComponent);
        }
    }

    public void HookControllerWithPawn(SimPawnControllerComponent controller, SimPawnComponent pawn)
    {
        if(controller.TargetPawn)
        {
            DebugService.LogError($"Cannot hook pawn '{pawn.gameObject.name}' with controller '{controller.gameObject}'." +
                (controller.TargetPawn ? $" The controller is already controlling '{controller.TargetPawn.gameObject.name}." : $"") +
                $" Please unhook before hooking again.");
            return;
        }

        // change pawn
        controller.TargetPawn = pawn;
        
        // notify
        NotifyPawnControllerOfPawnChange(controller.SimEntity);
    }

    public void UnhookControllerFromPawn(SimPawnControllerComponent controller)
    {
        controller.TargetPawn = null;

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

    List<ISimTargetPawnChangeListener> _cachedISimTargetPawnChangeListenerComponents = new List<ISimTargetPawnChangeListener>();

    void NotifyPawnControllerOfPawnChange(SimEntity controller)
    {
        controller.GetComponents<ISimTargetPawnChangeListener>(_cachedISimTargetPawnChangeListenerComponents);
        foreach (ISimTargetPawnChangeListener pawnChangeObserver in _cachedISimTargetPawnChangeListenerComponents)
        {
            pawnChangeObserver.OnTargetPawnChanged();
        }
    }
}