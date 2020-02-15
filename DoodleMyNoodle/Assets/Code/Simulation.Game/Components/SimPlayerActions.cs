using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerActions : SimClampedStatComponent, ISimTickable
{
    public int ActionGainByTurn = 1;

    private bool _wasMyTurn = false;

    public bool CanTakeAction()
    {
        return Value > 0 && SimTurnManager.Instance.IsMyTurn(Team.Player);
    }

    void ISimTickable.OnSimTick()
    {
        if (SimTurnManager.Instance.IsMyTurn(Team.Player))
        {
            if (!_wasMyTurn)
            {
                IncreaseValue(ActionGainByTurn);
                _wasMyTurn = true;
            }
        }
        else
        {
            _wasMyTurn = false;
        }
    }
}
