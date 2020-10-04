using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// fbessette: this script is temporary

public class StartSimUI : GameMonoBehaviour
{
    public SceneInfo SimManagersScene;
    public TMP_Dropdown levelToLoadField;
    public Button startSimButton;

    public LevelBank LevelBank;

    public override void OnGameStart()
    {
        base.OnGameStart();

        startSimButton.onClick.AddListener(OnStartClick);

        levelToLoadField.ClearOptions();
        List<string> levelOptions = new List<string>();
        foreach (Level level in LevelBank.Levels)
        {
            levelOptions.Add(level.name);
        }
        levelToLoadField.AddOptions(levelOptions);
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
        string levelName = levelToLoadField.options[levelToLoadField.value].text;
        PlayerPrefs.SetString("startLevel", levelName);
        PlayerPrefs.Save();
        LevelManager.Instance.StartLevel(levelName);
    }
}
