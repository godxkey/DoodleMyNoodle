using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      THIS IS UGLY TEMPORARY CODE                                 
    ////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] SceneInfo _escapeMenuScene;

    MenuInGameEscape _menuInGameEscape;

    void Start()
    {
        SceneService.LoadAsync(_escapeMenuScene, OnEscapeMenuSceneLoaded);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuInGameEscape?.Open();
        }
    }

    void OnEscapeMenuSceneLoaded(Scene scene)
    {
        _menuInGameEscape = scene.FindRootObject<MenuInGameEscape>();
    }
}
