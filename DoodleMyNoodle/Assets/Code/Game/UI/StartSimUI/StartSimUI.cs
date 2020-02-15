using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// fbessette: this script is temporary

public class StartSimUI : GameMonoBehaviour
{
    public SceneInfo SimManagersScene;
    public TMP_InputField levelToLoadField;
    public Button startSimButton;

    public override void OnGameStart()
    {
        base.OnGameStart();

        startSimButton.onClick.AddListener(OnStartClick);
        levelToLoadField.text = PlayerPrefs.GetString("startLevel", "");
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (LevelManager.Instance.IsLevelStarted)
        {
            gameObject.SetActive(false);
        }
    }

    void OnStartClick()
    {
        PlayerPrefs.SetString("startLevel", levelToLoadField.text);
        PlayerPrefs.Save();
        LevelManager.Instance.StartLevel(levelToLoadField.text);
    }
}
