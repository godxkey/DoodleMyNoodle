using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCC.InspectorDisplay;

public class SimPlayerActions : SimClampedStatComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        public bool WasMyTurn;
        public int ActionGainByTurn;
    }

    public int ActionGainByTurn { get => _data.ActionGainByTurn; set => _data.ActionGainByTurn = value; }

    public bool WasMyTurn { get => _data.WasMyTurn; internal set => _data.WasMyTurn = value; }

    public bool CanTakeAction(int Cost)
    {
        return ((Value - Cost) >= 0) && SimTurnManager.Instance.IsMyTurn(Team.Player);
    }

    void ISimTickable.OnSimTick()
    {
        // TODO : Changer pour global event
        if (SimTurnManager.Instance.IsMyTurn(Team.Player))
        {
            if (!WasMyTurn)
            {
                IncreaseValue(ActionGainByTurn);
                WasMyTurn = true;
            }
        }
        else
        {
            WasMyTurn = false;
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
