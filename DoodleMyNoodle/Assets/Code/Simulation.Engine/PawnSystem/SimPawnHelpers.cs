using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimPawnHelpers
{
    public static T GetComponentOnMainPlayer<T>(MonoBehaviour player) where T : SimComponent
    {
        if (player == null)
            return null;

        if (player.GetComponent(out SimPawnControllerComponent pawnController))
        {
            return pawnController.TargetPawn.gameObject.GetComponent<T>();
        }

        return null;
    }

    public static SimPawnComponent GetPawnFromController(MonoBehaviour player)
    {
        if (player == null)
            return null;

        if (player.GetComponent(out SimPawnControllerComponent pawnController))
        {
            return pawnController.TargetPawn;
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
