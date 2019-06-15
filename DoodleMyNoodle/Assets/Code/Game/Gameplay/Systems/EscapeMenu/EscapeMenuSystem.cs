using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuSystem : GameSystem<EscapeMenuSystem>
{
    public override bool isSystemReady => true;

    [SerializeField] SceneInfo _escapeMenuScene;

    MenuInGameEscape _menuInGameEscape;

    public override void OnGameReady()
    {
        base.OnGameReady();
        SceneService.LoadAsync(_escapeMenuScene.SceneName, LoadSceneMode.Additive, OnEscapeMenuSceneLoaded);
    }

    void OnEscapeMenuSceneLoaded(Scene scene)
    {
        _menuInGameEscape = scene.FindRootObject<MenuInGameEscape>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuInGameEscape?.Open();
        }
    }
}
