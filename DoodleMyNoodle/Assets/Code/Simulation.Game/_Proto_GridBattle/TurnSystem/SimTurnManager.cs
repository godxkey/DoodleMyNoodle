using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimTurnManager : SimComponent
{
    public int DurationOfATurn = 10;
    public Team StartTeam;

    private Fix64 _timer = 0;
    private Team _currentTeam;

    private bool _hasInit = false;

    public override void OnSimStart() 
    {
        _timer = DurationOfATurn;
        _currentTeam = StartTeam;
        _hasInit = true;
        Debug.Log("SWITCH TURN - " + _currentTeam);
    }

    void Update()
    {
        if (_hasInit)
        {
            _timer -= (Fix64)Time.deltaTime;

            if (_timer <= 0)
            {
                SwitchTurn();
                _timer = DurationOfATurn;
            }
        }
    }

    private void SwitchTurn()
    {
        _currentTeam = _currentTeam + 1;
        if((int)_currentTeam >= SimTeams.TEAM_COUNT)
        {
            _currentTeam = 0;
        }
        Debug.Log("SWITCH TURN - " + _currentTeam);
    }
}
