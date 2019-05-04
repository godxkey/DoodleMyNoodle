using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

[CreateAssetMenu(menuName = "DoodleMyNoodle/QuickStart Assets")]
public class QuickStartAssets : ScriptableObject
{
    public static QuickStartAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = (QuickStartAssets)Resources.Load("QuickStartAssets");

            return _instance;
        }
    }
    static QuickStartAssets _instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
}
