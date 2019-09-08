using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// fbessette: this script is temporary

public class StartSimUI : GameMonoBehaviour
{
    public TMP_InputField levelToLoadField;
    public Button startSimButton;

    public override void OnGameStart()
    {
        base.OnGameStart();

        string levelToPlay = null;

        switch (GameStateManager.currentGameState)
        {
            case GameStateInGameServer serverGameState:
                levelToPlay = serverGameState.LevelToPlay;
                break;

            case GameStateInGameLocal localGameState:
                levelToPlay = localGameState.LevelToPlay;
                break;
        }


        if (levelToPlay.IsNullOrEmpty() == false)
        {
            StartSimWithLevel(levelToPlay);
        }
        else
        {
            startSimButton.onClick.AddListener(OnStartClick);
            levelToLoadField.text = PlayerPrefs.GetString("startLevel", "");
        }
    }

    void OnStartClick()
    {
        StartSimWithLevel(levelToLoadField.text);
    }

    void StartSimWithLevel(string level)
    {
        ((SimulationControllerMaster)SimulationControllerMaster.Instance).AllowSimToTick = true;
        if (!level.IsNullOrEmpty())
        {
            SimulationControllerMaster.Instance.SubmitInput(new SimCommandLoadScene() { sceneName = level });
        }

        PlayerPrefs.SetString("startLevel", level);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}
