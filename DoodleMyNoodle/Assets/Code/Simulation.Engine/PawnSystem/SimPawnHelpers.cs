using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimPawnHelpers
{
    public static SimPawnComponent GetPawnFromPlayer(MonoBehaviour player)
    {
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
        return FindPawnController(pawn) == null;
    }

    public static SimEntity FindPawnController(SimPawnComponent pawn)
    {
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
