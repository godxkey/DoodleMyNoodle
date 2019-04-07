using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushCombatResolver : MonoBehaviour
{
    public struct CombatResult
    {
        public RushToy winner;
    }

    public CombatResult ResolveConflict(RushToy toyLeft, RushToy toyRight)
    {
        CombatResult currentResult = new CombatResult();
        if(toyLeft.power < toyRight.power)
        {
            currentResult.winner = toyRight;
        }
        else if(toyLeft.power > toyRight.power)
	    {
            currentResult.winner = toyLeft;
        }

        return currentResult;
    }
}
