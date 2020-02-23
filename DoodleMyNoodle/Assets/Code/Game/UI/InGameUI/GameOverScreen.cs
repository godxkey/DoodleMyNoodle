using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text WinnerDisplay;
    public GameObject Container;

    public void DisplayWinner(Team winningTeam)
    {
        WinnerDisplay.text = winningTeam + " wins !";
        Container.SetActive(true);
    }

    private void Update()
    {
        if (SimEndGameManager.Instance && SimEndGameManager.Instance.GameEnded && !Container.activeSelf)
        {
            DisplayWinner(SimEndGameManager.Instance.WinningTeam);
        }
    }
}
