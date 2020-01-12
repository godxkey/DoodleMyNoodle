using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public Text CurrentTeamName;
    public Text CurrentTime;

    private string _previousTime;

    void Update()
    {
        if(SimTurnManager.Instance != null)
        {
            // Timer
            int currentTime = (int)SimTurnManager.Instance.GetCurrentTime();
            CurrentTime.text = "" + currentTime;
            if (currentTime <= 3)
            {
                CurrentTime.color = Color.red;
            }
            else
            {
                CurrentTime.color = Color.black;
            }

            if (CurrentTime.text != _previousTime)
            {
                // fade text shortly
            }

            _previousTime = CurrentTime.text;

            // Team Text
            Team currentTeam = SimTurnManager.Instance.GetCurrentTeam();
            CurrentTeamName.text = "" + currentTeam;
            switch (currentTeam)
            {
                case Team.Player:
                    CurrentTeamName.color = Color.green;
                    break;
                case Team.AI:
                    CurrentTeamName.color = Color.red;
                    break;
                default:
                    break;
            }
        }
    }
}
