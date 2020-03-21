using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimHealthStatComponent : SimClampedStatComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        [CCC.InspectorDisplay.ReadOnlyAlways]
        public bool Invincible;
        public bool LastTurnWasMine;
    }

    public bool Invincible { get => _data.Invincible; set => _data.Invincible = value; }

    public void OnSimTick()
    {
        if (SimTurnManager.Instance.IsMyTurn(GetComponent<SimTeamMemberComponent>().Team)) 
        {
            if (!_data.LastTurnWasMine)
            {
                _data.LastTurnWasMine = true;
                TurnChanged(true);
            }
        }
        else
        {
            if (_data.LastTurnWasMine)
            {
                _data.LastTurnWasMine = false;
                TurnChanged(false);
            }
        }
    }

    private void TurnChanged(bool isMyTurn)
    {
        if (isMyTurn)
            Invincible = false;
    }

    public override int SetValue(int value)
    {
        if(Invincible)
            return 0;

        if (value <= 0)
        {
            Simulation.Destroy(SimEntity);
        }
        return base.SetValue(value);
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        Invincible = false
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
