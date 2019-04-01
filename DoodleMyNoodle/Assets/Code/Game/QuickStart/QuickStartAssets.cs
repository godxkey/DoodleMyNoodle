using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/QuickStart Assets")]
public class QuickStartAssets : ScriptableObject
{
    public SceneInfo emptyScene;

    [Header("GameStates")]
    public GameStateDefinitionInGameLocal inGameLocal;
    public GameStateDefinitionInGameClient inGameClient;
    public GameStateDefinitionInGameServer inGameServer;
    public GameStateDefinitionLobbyLocal lobbyLocal;
    public GameStateDefinitionLobbyServer lobbyServer;
    public GameStateDefinitionLobbyClient lobbyClient;
    public GameStateDefinitionRootMenu rootMenu;

    [Header("Settings")]
    public float searchForSeverTimeout = -1;
    public QuickStartSettings defaultSettings;

    public static QuickStartAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = (QuickStartAssets)Resources.Load("QuickStartSettings");

            return _instance;
        }
    }
    static QuickStartAssets _instance;

}
