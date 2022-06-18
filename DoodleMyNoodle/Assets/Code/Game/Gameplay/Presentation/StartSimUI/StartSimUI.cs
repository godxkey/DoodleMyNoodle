using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Serialization;

// fbessette: this script is temporary

public class StartSimUI : GameMonoBehaviour
{
    public SceneInfo SimManagersScene;
    
    [FormerlySerializedAs("levelToLoadField")]
    public TMP_Dropdown MapToLoadField;
    
    [FormerlySerializedAs("startSimButton")]
    public Button StartSimButton;
    
    [FormerlySerializedAs("LevelBank")]
    public MapBank MapBank;

    public override void OnGameStart()
    {
        base.OnGameStart();

        StartSimButton.onClick.AddListener(OnStartClick);

        MapToLoadField.ClearOptions();
        List<string> mapOptions = new List<string>();
        foreach (Map map in MapBank.Maps)
        {
            mapOptions.Add(map.name);
        }
        MapToLoadField.AddOptions(mapOptions);
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (MapManager.Instance.IsMapStarted)
        {
            gameObject.SetActive(false);
        }
    }

    void OnStartClick()
    {
        string mapName = MapToLoadField.options[MapToLoadField.value].text;
        PlayerPrefs.SetString("startMap", mapName);
        PlayerPrefs.Save();
        MapManager.Instance.StartMap(mapName);
    }
}
