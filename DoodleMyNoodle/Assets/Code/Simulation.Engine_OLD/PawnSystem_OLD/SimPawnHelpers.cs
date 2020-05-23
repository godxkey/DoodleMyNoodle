using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimPawnHelpers
{
    public static T GetComponentOnControllersPawn<T>(MonoBehaviour pawnController) where T : SimComponent
    {
        if (pawnController == null)
            return null;

        if (pawnController.TryGetComponent(out SimPawnControllerComponent pawnControllerComponent))
        {
            if (pawnControllerComponent.TargetPawn)
                return pawnControllerComponent.TargetPawn.gameObject.GetComponent<T>();
        }

        return null;
    }

    public static SimPawnComponent GetPawnFromController(MonoBehaviour pawnController)
    {
        if (pawnController == null)
            return null;

        if (pawnController.TryGetComponent(out SimPawnControllerComponent pawnControllerComponent))
        {
            return pawnControllerComponent.TargetPawn;
        }

        return null;
    }

    public static SimPawnComponent FindUncontrolledPawn()
    {
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            if (IsPawnControlled(pawn) == false)
            {
                return pawn;
            }
        }

        return null;
    }

    public static bool IsPawnControlled(SimPawnComponent pawn)
    {
        return FindPawnController(pawn) != null;
    }

    public static SimEntity FindPawnController(SimPawnComponent pawn)
    {
        if (pawn == null)
            return null;

        foreach (SimPawnControllerComponent controller in Simulation.EntitiesWithComponent<SimPawnControllerComponent>())
        {
            if (controller.TargetPawn == pawn)
            {
                return controller.SimEntity;
            }
        }

        return null;
    }
}
