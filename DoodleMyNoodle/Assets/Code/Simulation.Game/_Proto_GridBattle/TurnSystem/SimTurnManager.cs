using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    AI
}

public class SimTurnManager : SimSingleton<SimTurnManager>, ISimTickable
{
    public const int TEAM_COUNT = 2;


    [System.Serializable]
    struct SerializedData
    {
        public int DurationOfATurn;
        public Fix64 Timer;
        public Team CurrentTeam;
    }

    public int DurationOfATurn => _data.DurationOfATurn;

    public override void OnSimStart() 
    {
        base.OnSimStart();

        _data.Timer = DurationOfATurn;

        SwitchTurn();
    }

    void ISimTickable.OnSimTick()
    {
        _data.Timer -= Simulation.DeltaTime;

        if (_data.Timer <= 0)
        {
            SwitchTurn();
            _data.Timer = DurationOfATurn;
        }
    }

    private void SwitchTurn()
    {
        _data.CurrentTeam = _data.CurrentTeam + 1;
        if((int)_data.CurrentTeam >= TEAM_COUNT)
        {
            _data.CurrentTeam = 0;
        }
        Debug.Log("SWITCH TURN - " + _data.CurrentTeam);
    }

    public bool IsMyTurn(Team myTeam)
    {
        return _data.CurrentTeam == myTeam;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        DurationOfATurn = 3,
        CurrentTeam = (Team)TEAM_COUNT
    };

    public override void SerializeToDataStack(SimComponentDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.DeserializeFromDataStack(dataStack);
    }
    #endregion
}
