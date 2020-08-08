using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public class EscapeMenuSystem : GameSystem<EscapeMenuSystem>
{
    public override bool SystemReady => true;

    [SerializeField] SceneInfo _escapeMenuScene;

    MenuInGameEscape _menuInGameEscape;
    ISceneLoadPromise _loadPromise;

    public override void OnGameAwake()
    {
        base.OnGameAwake();
        _loadPromise = SceneService.LoadAsync(_escapeMenuScene.SceneName, LoadSceneMode.Additive);
        _loadPromise.OnComplete += OnEscapeMenuSceneLoaded;
    }

    void OnEscapeMenuSceneLoaded(ISceneLoadPromise sceneLoadPromise)
    {
        _menuInGameEscape = sceneLoadPromise.Scene.FindComponentOnRoots<MenuInGameEscape>();
        _loadPromise = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameConsole.IsOpen())
        {
            _menuInGameEscape?.Open();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_loadPromise != null)
            _loadPromise.OnComplete -= OnEscapeMenuSceneLoaded;
    }
}
