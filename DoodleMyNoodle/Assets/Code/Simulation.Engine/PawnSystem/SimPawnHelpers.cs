using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimPawnHelpers
{
    public static SimPawnInterfaceComponent GetPawnFromPlayer(MonoBehaviour player)
    {
        if (player.GetComponent(out SimTargetPawnComponent pawnController))
        {
            return pawnController.Target;
        }

        return null;
    }

    public static SimPawnInterfaceComponent FindUncontrolledPawn()
    {
        foreach (SimPawnInterfaceComponent pawn in Simulation.EntitiesWithComponent<SimPawnInterfaceComponent>())
        {
            if (IsPawnControlled(pawn) == false)
            {
                return pawn;
            }
        }

        return null;
    }

    public static bool IsPawnControlled(SimPawnInterfaceComponent pawn)
    {
        return FindPawnController(pawn) == null;
    }

    public static SimEntity FindPawnController(SimPawnInterfaceComponent pawn)
    {
        foreach (SimTargetPawnComponent controller in Simulation.EntitiesWithComponent<SimTargetPawnComponent>())
        {
            if (controller.Target == pawn)
            {
                return controller.SimEntity;
            }
        }

        return null;
    }
}
