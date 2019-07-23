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

        startSimButton.onClick.AddListener(OnStartClick);

        levelToLoadField.text = PlayerPrefs.GetString("startLevel", "");
    }

    void OnStartClick()
    {
        ((SimulationControllerServer)SimulationControllerServer.instance).allowSimToTick = true;
        if (levelToLoadField.text != "")
        {
            SimulationControllerServer.instance.SubmitInput(new SimCommandLoadScene() { sceneName = levelToLoadField.text });
        }

        PlayerPrefs.SetString("startLevel", levelToLoadField.text);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}
