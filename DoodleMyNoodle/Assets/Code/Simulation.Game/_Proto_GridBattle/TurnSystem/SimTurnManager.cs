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

    public int DurationOfATurn = 10;

    private Fix64 _timer = 0;
    private Team _currentTeam = (Team)TEAM_COUNT;

    public override void OnSimStart() 
    {
        base.OnSimStart();

        _timer = DurationOfATurn;

        SwitchTurn();
    }

    void ISimTickable.OnSimTick()
    {
        _timer -= Simulation.DeltaTime;

        if (_timer <= 0)
        {
            SwitchTurn();
            _timer = DurationOfATurn;
        }
    }

    private void SwitchTurn()
    {
        _currentTeam = _currentTeam + 1;
        if((int)_currentTeam >= TEAM_COUNT)
        {
            _currentTeam = 0;
        }
        Debug.Log("SWITCH TURN - " + _currentTeam);
    }

    public bool IsMyTurn(Team myTeam)
    {
        return _currentTeam == myTeam;
    }

    public Team GetCurrentTeam()
    {
        return _currentTeam;
    }

    public Fix64 GetCurrentTime()
    {
        return _timer;
    }
}
